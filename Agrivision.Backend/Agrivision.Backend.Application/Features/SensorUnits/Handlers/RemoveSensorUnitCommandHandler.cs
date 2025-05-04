using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class RemoveSensorUnitCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, ISensorUnitRepository sensorUnitRepository, ISensorUnitDeviceRepository sensorUnitDeviceRepository) : IRequestHandler<RemoveSensorUnitCommand, Result>
{
    public async Task<Result> Handle(RemoveSensorUnitCommand request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if the field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if the user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if the user has permission to remove (only owner and manager can remove)
        if (farmUserRole.FarmRole.Name != "Owner" && farmUserRole.FarmRole.Name != "Manager")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if the sensor unit exists
        var sensorUnit = await sensorUnitRepository.FindByIdAsync(request.SensorUnitId, cancellationToken);
        if (sensorUnit is null)
            return Result.Failure(SensorUnitErrors.SensorUnitNotFound);
        
        // check if the sensor unit belongs to the field
        if (sensorUnit.FieldId != request.FieldId)
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // mark as deleted
        sensorUnit.IsDeleted = true;
        sensorUnit.DeletedOn = DateTime.UtcNow;
        sensorUnit.DeletedById = request.RequesterId;
        sensorUnit.UpdatedById = request.RequesterId;
        sensorUnit.UpdatedOn = DateTime.UtcNow;
        
        // update the sensor unit
        await sensorUnitRepository.UpdateAsync(sensorUnit, cancellationToken);
        
        // mark sensor unit device as unassigned
        sensorUnit.Device.IsAssigned = false;
        sensorUnit.Device.AssignedAt = null;
        sensorUnit.Device.UpdatedById = request.RequesterId;
        sensorUnit.Device.UpdatedOn = DateTime.UtcNow;
        
        // update the sensor unit device
        await sensorUnitDeviceRepository.UpdateAsync(sensorUnit.Device, cancellationToken);
        
        return Result.Success();
    }
}