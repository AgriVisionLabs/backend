using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.DiseaseDetection.Commands;
using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.DiseaseDetection;
using Agrivision.Backend.Application.Services.Files;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Handlers;

public class AddDiseaseDetectionsCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository, IFileUploadService fileUploadService, IDiseaseDetectionService diseaseDetectionService, ICropDiseaseRepository cropDiseaseRepository, ICropRepository cropRepository, IOptions<ServerSettings> serverSettings, IDiseaseDetectionRepository diseaseDetectionRepository, ILogger<AddDiseaseDetectionsCommandHandler> logger) : IRequestHandler<AddDiseaseDetectionCommand, Result<DiseaseDetectionResponse>>
{
    public async Task<Result<DiseaseDetectionResponse>> Handle(AddDiseaseDetectionCommand request, CancellationToken cancellationToken)
    {
        // verify field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<DiseaseDetectionResponse>(FieldErrors.FieldNotFound);

        // check if field belongs to farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<DiseaseDetectionResponse>(FarmErrors.UnauthorizedAction);

        // check if user has access
        var farmUserRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.ReqeusterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<DiseaseDetectionResponse>(FarmErrors.UnauthorizedAction);

        // expert can't access the disease detection feature
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<DiseaseDetectionResponse>(FarmUserRoleErrors.InsufficientPermissions);

        // check if the field has any planted crop
        if (field.PlantedCrop is null)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.DiseaseDetectionNotAllowedInEmptyField);

        // check if the crop supports disease detection
        var crop = await cropRepository.FindByIdAsync(field.PlantedCrop!.CropId, cancellationToken);
        if (crop is null)
            return Result.Failure<DiseaseDetectionResponse>(CropErrors.CropNotFound);

        if (!crop.SupportsDiseaseDetection)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.CropNotSupportedForDiseaseDetection);

        // upload image or video
        string filename;
        bool isVideo;

        if (request.Image is not null)
        {
            filename = await fileUploadService.UploadImageAsync(request.Image);
            isVideo = false;
        }
        else if (request.Video is not null)
        {
            filename = await fileUploadService.UploadVideoAsync(request.Video);
            isVideo = true;
        }
        else
        {
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);
        }

        if (string.IsNullOrWhiteSpace(filename))
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);
        
        // get user full name once for both paths
        var user = await userRepository.FindByIdAsync(request.ReqeusterId);
        if (user is null)
            return Result.Failure<DiseaseDetectionResponse>(UserErrors.UserNotFound);

        var userFullName = $"{user.FirstName} {user.LastName}";

        // process prediction based on file type
        string imageUrl;
        double confidenceValue;
        bool isHealthy;
        string diseaseName;
        Guid? cropDiseaseId = null;
        List<string> treatments = [];

        if (isVideo)
        {
            var (compositeImageUrl, healthyCount, infectedCount) = await diseaseDetectionService.PredictVideoAsync(filename);
            if (string.IsNullOrWhiteSpace(compositeImageUrl) || (healthyCount + infectedCount == 0))
                return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);

            var totalFrames = healthyCount + infectedCount;
            imageUrl = compositeImageUrl;
            confidenceValue = (double)healthyCount / totalFrames;
            isHealthy = confidenceValue >= 0.55;
            diseaseName = "Composite";
        }
        else
        {
            var prediction = await diseaseDetectionService.PredictImageAsync(filename);
            if (prediction is null || string.IsNullOrWhiteSpace(prediction.Label))
                return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);

            var label = prediction.Label.Trim();
            var matchedConfidence = prediction.Confidences.FirstOrDefault(c => c.Label == label);
            if (matchedConfidence is null)
                return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.ConfidenceLabelMismatch);

            confidenceValue = matchedConfidence.Confidence;
            isHealthy = label.Contains("healthy", StringComparison.OrdinalIgnoreCase);
            imageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}";

            var cropDisease = !isHealthy
                ? await cropDiseaseRepository.FindByNameAsync(label, cancellationToken)
                : null;

            if (!isHealthy && cropDisease is null)
                return Result.Failure<DiseaseDetectionResponse>(CropDiseaseErrors.CropDiseaseNotFound);

            diseaseName = isHealthy ? "Healthy" : cropDisease!.Name;
            cropDiseaseId = cropDisease?.Id;
            treatments = isHealthy ? [] : cropDisease!.Treatments;
        }

        // determine health status based on confidence and health
        var healthStatus = confidenceValue < 0.55
            ? PlantHealthStatus.AtRisk
            : isHealthy
                ? PlantHealthStatus.Healthy
                : PlantHealthStatus.Infected;

        // create and save detection entity
        var detection = new Domain.Entities.Core.DiseaseDetection
        {
            Id = Guid.NewGuid(),
            ConfidenceLevel = confidenceValue,
            ImageUrl = imageUrl,
            IsHealthy = isHealthy,
            HealthStatus = healthStatus,
            PlantedCrop = field.PlantedCrop!,
            CropDiseaseId = cropDiseaseId,
            CreatedById = request.ReqeusterId,
            CreatedOn = DateTime.UtcNow
        };

        await diseaseDetectionRepository.AddAsync(detection, cancellationToken);

        // return unified response
        return Result.Success(new DiseaseDetectionResponse(
            Id: detection.Id,
            FarmId: field.FarmId,
            FieldId: field.Id,
            CropName: crop.Name,
            DiseaseName: diseaseName,
            IsHealthy: isHealthy,
            HealthStatus: healthStatus,
            CreatedOn: detection.CreatedOn,
            ConfidenceLevel: detection.ConfidenceLevel,
            ImageUrl: detection.ImageUrl,
            CreatedBy: userFullName,
            Treatments: treatments
        ));
    }
}