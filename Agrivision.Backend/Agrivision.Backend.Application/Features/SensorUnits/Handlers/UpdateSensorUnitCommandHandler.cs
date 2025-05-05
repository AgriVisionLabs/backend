using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class UpdateSensorUnitCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, ISensorUnitRepository sensorUnitRepository) : IRequestHandler<UpdateSensorUnitCommand, Result>
{
    public async Task<Result> Handle(UpdateSensorUnitCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdAsync(request.NewFieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if the user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if the user has permission to update (only expert can't)
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // check if the sensor unit exists
        var sensorUnit = await sensorUnitRepository.FindByIdAsync(request.SensorUnitId, cancellationToken);
        if (sensorUnit is null)
            return Result.Failure(SensorUnitErrors.SensorUnitNotFound);
        
        // check if new field exists
        if (request.NewFieldId != sensorUnit.FieldId)
        {
            var newField = await fieldRepository.FindByIdAsync(request.NewFieldId, cancellationToken);
            if (newField is null || newField.FarmId != request.FarmId)
                return Result.Failure(FieldErrors.FieldNotFound);

            sensorUnit.FieldId = newField.Id;
        }
        
        // check if the farm doesn't have a unit with the same name
        var existing =
            await sensorUnitRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existing is not null && existing.FieldId != sensorUnit.FieldId)
            return Result.Failure(SensorUnitErrors.DuplicateNameInFarm);
        
        // update the sensor unit
        sensorUnit.Name = request.Name;
        sensorUnit.Status = request.Status;
        sensorUnit.UpdatedById = request.RequesterId;
        sensorUnit.UpdatedOn = DateTime.UtcNow;
        
        // update
        await sensorUnitRepository.UpdateAsync(sensorUnit, cancellationToken);
        
        return Result.Success();
    }
}