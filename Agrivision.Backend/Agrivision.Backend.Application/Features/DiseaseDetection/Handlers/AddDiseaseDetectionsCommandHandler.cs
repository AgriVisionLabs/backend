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

public class AddDiseaseDetectionsCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository, IFileService fileService, IDiseaseDetectionService diseaseDetectionService, ICropDiseaseRepository cropDiseaseRepository, ICropRepository cropRepository, IOptions<ServerSettings> serverSettings, IDiseaseDetectionRepository diseaseDetectionRepository, ILogger<AddDiseaseDetectionsCommandHandler> logger) : IRequestHandler<AddDiseaseDetectionCommand, Result<DiseaseDetectionResponse>>
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

        try
        {
            if (request.Image is not null)
            {
                filename = await fileService.UploadImageAsync(request.Image);
                isVideo = false;
            }
            else if (request.Video is not null)
            {
                filename = await fileService.UploadVideoAsync(request.Video);
                isVideo = true;
            }
            else
            {
                return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);
            }

            if (string.IsNullOrWhiteSpace(filename))
                return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to upload file for farm {FarmId}, field {FieldId}", request.FarmId, request.FieldId);
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);
        }

        // get user full name once for both paths
        var user = await userRepository.FindByIdAsync(request.ReqeusterId);
        if (user is null)
            return Result.Failure<DiseaseDetectionResponse>(UserErrors.UserNotFound);

        var userFullName = $"{user.FirstName} {user.LastName}";

        return isVideo
            ? await HandleVideoAsync(field, crop, filename, userFullName, request, cancellationToken)
            : await HandleImageAsync(field, crop, filename, userFullName, request, cancellationToken);
    }
    
    private async Task<Result<DiseaseDetectionResponse>> HandleVideoAsync(
        Field field, Domain.Entities.Core.Crop crop, string filename, string userFullName,
        AddDiseaseDetectionCommand request, CancellationToken cancellationToken)
    {
        // video processing path
        var (compositeImageUrl, healthyCount, infectedCount) = await diseaseDetectionService.PredictVideoAsync(filename);
        if (string.IsNullOrWhiteSpace(compositeImageUrl) || (healthyCount + infectedCount == 0))
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);

        var totalFrames = healthyCount + infectedCount;
        var confidenceValue = (double)healthyCount / totalFrames;
        var isHealthy = confidenceValue >= 0.55;
        var healthStatus = isHealthy ? PlantHealthStatus.Healthy : PlantHealthStatus.Infected;

        var detection = new Domain.Entities.Core.DiseaseDetection
        {
            Id = Guid.NewGuid(),
            ConfidenceLevel = confidenceValue,
            ImageUrl = compositeImageUrl,
            IsHealthy = isHealthy,
            HealthStatus = healthStatus,
            PlantedCrop = field.PlantedCrop,
            CropDiseaseId = null,
            CreatedById = request.ReqeusterId,
            CreatedOn = DateTime.UtcNow
        };

        await diseaseDetectionRepository.AddAsync(detection, cancellationToken);

        return Result.Success(new DiseaseDetectionResponse(
            Id: detection.Id,
            FarmId: field.FarmId,
            FieldId: field.Id,
            CropName: crop.Name,
            DiseaseName: "Composite",
            IsHealthy: isHealthy,
            HealthStatus: healthStatus,
            CreatedOn: detection.CreatedOn,
            ConfidenceLevel: detection.ConfidenceLevel,
            ImageUrl: detection.ImageUrl,
            CreatedBy: userFullName,
            Treatments: []
        ));
    }
    private async Task<Result<DiseaseDetectionResponse>> HandleImageAsync(
    Field field, Domain.Entities.Core.Crop crop, string filename, string userFullName,
    AddDiseaseDetectionCommand request, CancellationToken cancellationToken)
    {
        var prediction = await diseaseDetectionService.PredictImageAsync(filename);
        if (prediction is null || string.IsNullOrWhiteSpace(prediction.Label))
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);

        var label = prediction.Label.Trim();
        var matchedConfidence = prediction.Confidences.FirstOrDefault(c => c.Label == label);
        if (matchedConfidence is null)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.ConfidenceLabelMismatch);

        var confidenceValue = matchedConfidence.Confidence;
        var isHealthy = label.Contains("healthy", StringComparison.OrdinalIgnoreCase);

        var cropDisease = !isHealthy
            ? await cropDiseaseRepository.FindByNameAsync(label, cancellationToken)
            : null;

        if (!isHealthy && cropDisease is null)
            return Result.Failure<DiseaseDetectionResponse>(CropDiseaseErrors.CropDiseaseNotFound);

        var healthStatus = confidenceValue < 0.55
            ? PlantHealthStatus.AtRisk
            : isHealthy
                ? PlantHealthStatus.Healthy
                : PlantHealthStatus.Infected;

        var imageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}";

        var detection = new Domain.Entities.Core.DiseaseDetection
        {
            Id = Guid.NewGuid(),
            ConfidenceLevel = confidenceValue,
            ImageUrl = imageUrl,
            IsHealthy = isHealthy,
            HealthStatus = healthStatus,
            PlantedCrop = field.PlantedCrop,
            CropDiseaseId = cropDisease?.Id,
            CreatedById = request.ReqeusterId,
            CreatedOn = DateTime.UtcNow
        };

        await diseaseDetectionRepository.AddAsync(detection, cancellationToken);

        return Result.Success(new DiseaseDetectionResponse(
            Id: detection.Id,
            FarmId: field.FarmId,
            FieldId: field.Id,
            CropName: crop.Name,
            DiseaseName: isHealthy ? "Healthy" : cropDisease!.Name,
            IsHealthy: isHealthy,
            HealthStatus: healthStatus,
            CreatedOn: detection.CreatedOn,
            ConfidenceLevel: detection.ConfidenceLevel,
            ImageUrl: detection.ImageUrl,
            CreatedBy: userFullName,
            Treatments: isHealthy ? [] : cropDisease!.Treatments
        ));
    }
    
}