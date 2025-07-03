using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.DiseaseDetection.Commands;
using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.DiseaseDetection;
using Agrivision.Backend.Application.Services.Files;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Handlers;

public class AddDiseaseDetectionCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository, IFileUploadService fileUploadService, IDiseaseDetectionService diseaseDetectionService, ICropDiseaseRepository cropDiseaseRepository, ICropRepository cropRepository, IOptions<ServerSettings> serverSettings, IDiseaseDetectionRepository diseaseDetectionRepository) : IRequestHandler<AddDiseaseDetectionCommand, Result<DiseaseDetectionResponse>>
{
    public async Task<Result<DiseaseDetectionResponse>> Handle(AddDiseaseDetectionCommand request, CancellationToken cancellationToken)
    {
        // verify field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<DiseaseDetectionResponse>(FieldErrors.FieldNotFound);

        // check if field belong to farm 
        if (field.FarmId != request.FarmId)
            return Result.Failure<DiseaseDetectionResponse>(FarmErrors.UnauthorizedAction);

        // check if user has access
        var farmUserRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(
            request.ReqeusterId, request.FarmId, cancellationToken);
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

        // upload image
        var filename = await fileUploadService.UploadImageAsync(request.Image);

        // disease detection
        var prediction = await diseaseDetectionService.PredictImageAsync(filename);
        if (prediction is null || string.IsNullOrWhiteSpace(prediction.Label))
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.PredictionFailed);

        var label = prediction.Label.Trim();
        var isHealthy = label.Contains("healthy", StringComparison.OrdinalIgnoreCase);

        // find the crop disease with the highest confidence
        var cropDisease = !isHealthy
            ? await cropDiseaseRepository.FindByNameAsync(label, cancellationToken)
            : null;

        if (!isHealthy && cropDisease is null)
            return Result.Failure<DiseaseDetectionResponse>(CropDiseaseErrors.CropDiseaseNotFound);

        // get username
        var user = await userRepository.FindByIdAsync(request.ReqeusterId);
        if (user is null)
            return Result.Failure<DiseaseDetectionResponse>(UserErrors.UserNotFound);

        var userFullName = user.FirstName + " " + user.LastName;

        // match prediction confidence
        var matchedConfidence = prediction.Confidences
            .FirstOrDefault(c => c.Label == label);

        if (matchedConfidence == null)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.ConfidenceLabelMismatch);

        var confidenceValue = matchedConfidence.Confidence;

        // create new disease detection
        var diseaseDetection = new Domain.Entities.Core.DiseaseDetection
        {
            Id = Guid.NewGuid(),
            ConfidenceLevel = confidenceValue,
            ImageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}",
            PlantedCropId = field.PlantedCrop.Id,
            CropDiseaseId = cropDisease?.Id, // allow null for healthy plants
            CreatedById = request.ReqeusterId,
            CreatedOn = DateTime.UtcNow
        };

        await diseaseDetectionRepository.AddAsync(diseaseDetection, cancellationToken);

        // map
        var response = new DiseaseDetectionResponse(
            Id: diseaseDetection.Id,
            FarmId: field.FarmId,
            FieldId: field.Id,
            CropName: crop.Name,
            DiseaseName: isHealthy ? "Healthy" : cropDisease!.Name,
            IsHealthy: isHealthy,
            CreatedOn: diseaseDetection.CreatedOn,
            ConfidenceLevel: diseaseDetection.ConfidenceLevel,
            ImageUrl: diseaseDetection.ImageUrl,
            CreatedBy: userFullName,
            Treatments: isHealthy ? [] : cropDisease!.Treatments
        );

        return Result.Success(response);
    }
}