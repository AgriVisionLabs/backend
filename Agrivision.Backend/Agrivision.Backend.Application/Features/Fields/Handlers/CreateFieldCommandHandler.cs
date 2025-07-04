using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Features.Fields.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class CreateFieldCommandHandler (IFieldRepository fieldRepository, IFarmRepository farmRepository, ICropRepository cropRepository, IPlantedCropRepository plantedCropRepository) : IRequestHandler<CreateFieldCommand, Result<FieldResponse>>
{
    public async Task<Result<FieldResponse>> Handle(CreateFieldCommand request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdWithFieldsAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<FieldResponse>(FarmErrors.FarmNotFound);
        
        // check if farm owner
        if (farm.CreatedById != request.CreatedById)
            return Result.Failure<FieldResponse>(FieldErrors.UnauthorizedAction);
        
        // check if field name is not used 
        var existingField =
            await fieldRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existingField is not null)
            return Result.Failure<FieldResponse>(FieldErrors.DuplicateFieldName);
        
        // calculate used area
        var usedArea = farm.Fields
            .Where(f => !f.IsDeleted)
            .Sum(f => f.Area); 
        
        // check if field area is appropriate
        if (usedArea + request.Area > farm.Area)
            return Result.Failure<FieldResponse>(FieldErrors.InvalidFieldArea);
        
        // map to field
        var field = new Field
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Area = request.Area,
            IsActive = false,
            FarmId = farm.Id,
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.CreatedById,
            IsDeleted = false
        };
        
        // check if specified a crop
        Domain.Entities.Core.Crop? crop = null;
        PlantedCrop? plantedCrop = null;
        if (request.CropType is not null)
        {
            crop = await cropRepository.FindByCropTypeAsync(request.CropType.Value, cancellationToken);
            if (crop is null)
                return Result.Failure<FieldResponse>(CropErrors.CropNotFound);

            plantedCrop = new PlantedCrop
            {
                Id = Guid.NewGuid(),
                PlantingDate = DateTime.UtcNow,
                ExpectedHarvestDate = DateTime.UtcNow.AddDays(crop.GrowthDurationDays),
                CropId = crop.Id,
                FieldId = field.Id,
                CreatedById = request.CreatedById,
                CreatedOn = DateTime.UtcNow
            };

            field.PlantedCropId = plantedCrop.Id;
        }

        farm.FieldsNo++;

        await farmRepository.UpdateAsync(farm, cancellationToken);

        await fieldRepository.AddAsync(field, cancellationToken);

        if (plantedCrop is not null)
            await plantedCropRepository.AddAsync(plantedCrop, cancellationToken);

        return Result.Success(new FieldResponse(field.Id, field.Name, field.Area, field.IsActive, field.FarmId, crop?.CropType, crop?.Name, crop?.Description, crop?.SupportsDiseaseDetection, plantedCrop?.PlantingDate, 0, plantedCrop?.ExpectedHarvestDate));
    }
}