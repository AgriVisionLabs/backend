using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Application.Features.Fields.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class GetFieldByIdQueryHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IPlantedCropRepository plantedCropRepository) : IRequestHandler<GetFieldByIdQuery, Result<FieldResponse>>
{
    public async Task<Result<FieldResponse>> Handle(GetFieldByIdQuery request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<FieldResponse>(FieldErrors.FieldNotFound);
        
        // check if user has access to the farm
        var hasAccess = await farmUserRoleRepository.ExistsAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (!hasAccess) 
            return Result.Failure<FieldResponse>(FarmErrors.UnauthorizedAction);
        
        // map to a response
        CropType? cropType = null;
        string? cropName = null;
        string? description = null;
        bool? supportsDiseaseDetection = null;
        DateTime? plantingDate = null;
        DateTime? expectedHarvestDate = null;
        int? progress = null;

        // fetch latest planted crop for this field
        var plantedCrop = await plantedCropRepository.FindLatestByFieldId(request.FieldId, cancellationToken);
        if (plantedCrop is not null)
        {
            cropType = plantedCrop.Crop.CropType;
            cropName = plantedCrop.Crop.Name;
            description = plantedCrop.Crop.Description;
            supportsDiseaseDetection = plantedCrop.Crop.SupportsDiseaseDetection;
            plantingDate = plantedCrop.PlantingDate;
            expectedHarvestDate = plantedCrop.ExpectedHarvestDate;

            // safe progress calculation
            if (plantedCrop.Crop.GrowthDurationDays > 0)
            {
                var daysPassed = (DateTime.UtcNow - plantedCrop.PlantingDate).TotalDays;
                var rawProgress = (daysPassed / plantedCrop.Crop.GrowthDurationDays) * 100;
                progress = (int)Math.Clamp(rawProgress, 0, 100);
            }
        }
        
        var response = new FieldResponse(
            field.Id,
            field.Name,
            field.Area,
            field.IsActive,
            field.FarmId,
            cropType,
            cropName,
            description,
            supportsDiseaseDetection,
            plantingDate,
            progress,
            expectedHarvestDate
        );

        return Result.Success(response);
    }
}