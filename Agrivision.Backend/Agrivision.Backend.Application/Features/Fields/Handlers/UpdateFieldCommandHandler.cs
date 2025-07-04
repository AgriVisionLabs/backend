using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Fields.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Fields.Handlers;

public class UpdateFieldCommandHandler(IFieldRepository fieldRepository, IFarmRepository farmRepository, IPlantedCropRepository plantedCropRepository, ICropRepository cropRepository) : IRequestHandler<UpdateFieldCommand, Result>
{
    public async Task<Result> Handle(UpdateFieldCommand request, CancellationToken cancellationToken)
    {
        // check if field exist
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // confirm field belongs to provided farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // fetch the farm
        var farm = await farmRepository.FindByIdWithFieldsAsync(field.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // check for duplicate field name in the same farm
        var existingField =
            await fieldRepository.FindByNameAndFarmIdAsync(request.Name, farm.Id, cancellationToken);
        if (existingField is not null && existingField.Id != field.Id && !existingField.IsDeleted)
            return Result.Failure(FieldErrors.DuplicateFieldName);
        
        // only owner can update fields (and farms)
        if (farm.CreatedById != request.UpdatedById)
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // calculate used area
        var usedArea = farm.Fields
            .Where(f => !f.IsDeleted && f.Id != field.Id) // to get the sum of all and not include the one we are updating 
            .Sum(f => f.Area);
        
        // check if field area is appropriate
        if (usedArea + request.Area > farm.Area)
            return Result.Failure(FieldErrors.InvalidFieldArea);


        // update
        field.Name = request.Name;
        field.Area = request.Area;
        field.UpdatedOn = DateTime.UtcNow;
        field.UpdatedById = request.UpdatedById;
        
        // update crop
        var newCropType = request.CropType;
        var currentPlantedCrop = await plantedCropRepository.FindLatestByFieldId(field.Id, cancellationToken);

        // new crop assigned when none was before
        if (newCropType is not null && currentPlantedCrop is null)
        {
            var crop = await cropRepository.FindByCropTypeAsync(newCropType.Value, cancellationToken);
            if (crop is null)
                return Result.Failure(CropErrors.CropNotFound);

            var newPlantedCrop = new PlantedCrop
            {
                Id = Guid.NewGuid(),
                FieldId = field.Id,
                CropId = crop.Id,
                PlantingDate = DateTime.UtcNow,
                ExpectedHarvestDate = DateTime.UtcNow.AddDays(crop.GrowthDurationDays),
                CreatedById = request.UpdatedById,
                CreatedOn = DateTime.UtcNow
            };

            field.PlantedCropId = newPlantedCrop.Id;
            await plantedCropRepository.AddAsync(newPlantedCrop, cancellationToken);
        }

        // crop changed (A → B)
        else if (newCropType is not null && currentPlantedCrop is not null)
        {
            var newCrop = await cropRepository.FindByCropTypeAsync(newCropType.Value, cancellationToken);
            if (newCrop is null)
                return Result.Failure(CropErrors.CropNotFound);

            if (currentPlantedCrop.CropId != newCrop.Id)
            {
                // finish the previous crop
                currentPlantedCrop.ActualHarvestDate = DateTime.UtcNow;
                currentPlantedCrop.UpdatedById = request.UpdatedById;
                currentPlantedCrop.UpdatedOn = DateTime.UtcNow;
                await plantedCropRepository.UpdateAsync(currentPlantedCrop, cancellationToken);

                // plant the new crop
                var newPlantedCrop = new PlantedCrop
                {
                    Id = Guid.NewGuid(),
                    FieldId = field.Id,
                    CropId = newCrop.Id,
                    PlantingDate = DateTime.UtcNow,
                    ExpectedHarvestDate = DateTime.UtcNow.AddDays(newCrop.GrowthDurationDays),
                    CreatedById = request.UpdatedById,
                    CreatedOn = DateTime.UtcNow
                };
                
                field.PlantedCropId = newPlantedCrop.Id;
                await plantedCropRepository.AddAsync(newPlantedCrop, cancellationToken);
            }
        }

        // crop removed (A → null)
        else if (newCropType is null && currentPlantedCrop is not null)
        {
            currentPlantedCrop.IsDeleted = true;
            currentPlantedCrop.DeletedOn = DateTime.UtcNow;
            currentPlantedCrop.DeletedById = request.UpdatedById;
            currentPlantedCrop.UpdatedById = request.UpdatedById;
            currentPlantedCrop.UpdatedOn = DateTime.UtcNow;
            field.PlantedCropId = null; // remove the reference to the planted crop

            await plantedCropRepository.UpdateAsync(currentPlantedCrop, cancellationToken);
        }
        
        // save
        await fieldRepository.UpdateAsync(field, cancellationToken);

        return Result.Success();
    }
}