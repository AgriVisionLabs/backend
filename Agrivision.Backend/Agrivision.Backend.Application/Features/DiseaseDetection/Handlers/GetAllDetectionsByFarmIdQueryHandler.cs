using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.DiseaseDetection.Contracts;
using Agrivision.Backend.Application.Features.DiseaseDetection.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.DiseaseDetection.Handlers;

public class GetAllDetectionsByFarmIdQueryHandler(IDiseaseDetectionRepository diseaseDetectionRepository, IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository, ICropRepository cropRepository) : IRequestHandler<GetAllDetectionsByFarmIdQuery, Result<IReadOnlyList<DiseaseDetectionResponse>>>
{
    public async Task<Result<IReadOnlyList<DiseaseDetectionResponse>>> Handle(GetAllDetectionsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<DiseaseDetectionResponse>>(FarmErrors.FarmNotFound);

        // check if user has access to the farm
        var farmUserRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<DiseaseDetectionResponse>>(FarmErrors.UnauthorizedAction);

        // get all disease detections for the farm
        var diseaseDetections = await diseaseDetectionRepository.GetAllByFarmIdAsync(request.FarmId, cancellationToken);

        // collect user ids 
        var createdByUserIds = diseaseDetections
            .Select(dd => dd.CreatedById)
            .Distinct()
            .ToList();

        var userDict = new Dictionary<string, string>();
        foreach (var userId in createdByUserIds)
        {
            var user = await userRepository.FindByIdAsync(userId);
            if (user != null)
            {
                userDict[userId] = $"{user.FirstName} {user.LastName}";
            }
        }

        // map to response
        var responses = diseaseDetections.Select(dd =>
        {
            var plantedCrop = dd.PlantedCrop;
            var crop = plantedCrop.Crop;
            var ddField = plantedCrop.Field;
            var disease = dd.CropDisease;

            var userFullName = userDict.TryGetValue(dd.CreatedById, out var name)
                ? name
                : "Unknown";

            return new DiseaseDetectionResponse(
                Id: dd.Id,
                FarmId: ddField?.FarmId ?? Guid.Empty,
                FieldId: ddField?.Id ?? Guid.Empty,
                CropName: crop?.Name ?? "Unknown Crop",
                DiseaseName: dd.IsHealthy ? "Healthy" : (disease?.Name ?? "Unknown Disease"),
                IsHealthy: dd.IsHealthy,
                HealthStatus: dd.HealthStatus,
                CreatedOn: dd.CreatedOn,
                ConfidenceLevel: dd.ConfidenceLevel,
                ImageUrl: dd.ImageUrl,
                CreatedBy: userFullName,
                Treatments: dd.IsHealthy ? [] : (disease?.Treatments ?? new List<string>())
            );
        }).ToList();

        return Result.Success<IReadOnlyList<DiseaseDetectionResponse>>(responses);
    }
}