using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.SensorUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Handlers;

public class RemoveSensorUnitCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, ISensorUnitRepository sensorUnitRepository, ISensorUnitDeviceRepository sensorUnitDeviceRepository, IAutomationRuleRepository automationRuleRepository) : IRequestHandler<RemoveSensorUnitCommand, Result>
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
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
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
        
        var now = DateTime.UtcNow;
        
        // get all automation rules for this farm and delete threshold rules using this sensor unit
        var farmAutomationRules = await automationRuleRepository.FindByFarmIdAsync(request.FarmId, cancellationToken);
        var rulesToDelete = farmAutomationRules.Where(r => !r.IsDeleted && 
                                                           r.Type == AutomationRuleType.Threshold && 
                                                           r.SensorUnitId == sensorUnit.Id).ToList();
        foreach (var rule in rulesToDelete)
        {
            rule.IsDeleted = true;
            rule.DeletedOn = now;
            rule.DeletedById = request.RequesterId;
            
            await automationRuleRepository.UpdateAsync(rule, cancellationToken);
        }
        
        // mark as deleted
        sensorUnit.IsDeleted = true;
        sensorUnit.DeletedOn = now;
        sensorUnit.DeletedById = request.RequesterId;
        sensorUnit.UpdatedById = request.RequesterId;
        sensorUnit.UpdatedOn = now;
        
        // update the sensor unit
        await sensorUnitRepository.UpdateAsync(sensorUnit, cancellationToken);
        
        // mark sensor unit device as unassigned
        sensorUnit.Device.IsAssigned = false;
        sensorUnit.Device.AssignedAt = null;
        sensorUnit.Device.UpdatedById = request.RequesterId;
        sensorUnit.Device.UpdatedOn = now;
        
        // update the sensor unit device
        await sensorUnitDeviceRepository.UpdateAsync(sensorUnit.Device, cancellationToken);
        
        return Result.Success();
    }
}