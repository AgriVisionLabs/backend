using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Application.Features.DiseaseDetection.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Handlers;

public class GetDetectionByIdQueryHandler(IDiseaseDetectionRepository diseaseDetectionRepository, IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository) : IRequestHandler<GetDetectionByIdQuery, Result<DiseaseDetectionResponse>>
{
    public async Task<Result<DiseaseDetectionResponse>> Handle(GetDetectionByIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<DiseaseDetectionResponse>(FarmErrors.FarmNotFound);

        // check if user has access to the farm
        var farmUserRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<DiseaseDetectionResponse>(FarmErrors.UnauthorizedAction);

        // get the disease detection
        var diseaseDetection = await diseaseDetectionRepository.FindByIdAsync(request.DetectionId, cancellationToken);
        if (diseaseDetection is null)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.DiseaseDetectionNotFound);

        // check if the detection belongs to the farm (through PlantedCrop -> Field -> Farm)
        if (diseaseDetection.PlantedCrop.Field.FarmId != request.FarmId)
            return Result.Failure<DiseaseDetectionResponse>(DiseaseDetectionErrors.DiseaseDetectionNotFound);

        // get user info
        var user = await userRepository.FindByIdAsync(diseaseDetection.CreatedById);
        var userFullName = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown";

        // map to response
        var plantedCrop = diseaseDetection.PlantedCrop;
        var crop = plantedCrop.Crop;
        var field = plantedCrop.Field;
        var disease = diseaseDetection.CropDisease;

        var response = new DiseaseDetectionResponse(
            Id: diseaseDetection.Id,
            FarmId: field?.FarmId ?? Guid.Empty,
            FieldId: field?.Id ?? Guid.Empty,
            CropName: crop?.Name ?? "Unknown Crop",
            DiseaseName: diseaseDetection.IsHealthy ? "Healthy" : (disease?.Name ?? "Unknown Disease"),
            IsHealthy: diseaseDetection.IsHealthy,
            HealthStatus: diseaseDetection.HealthStatus,
            CreatedOn: diseaseDetection.CreatedOn,
            ConfidenceLevel: diseaseDetection.ConfidenceLevel,
            ImageUrl: diseaseDetection.ImageUrl,
            CreatedBy: userFullName,
            Treatments: diseaseDetection.IsHealthy ? [] : (disease?.Treatments ?? new List<string>())
        );

        return Result.Success(response);
    }
}