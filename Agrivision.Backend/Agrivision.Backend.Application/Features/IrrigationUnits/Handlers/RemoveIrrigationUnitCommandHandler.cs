using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.IrrigationUnits.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Handlers;

public class RemoveIrrigationUnitCommandHandler(IFieldRepository fieldRepository, IFarmUserRoleRepository farmUserRoleRepository, IIrrigationUnitRepository irrigationUnitRepository, IIrrigationUnitDeviceRepository irrigationUnitDeviceRepository, IAutomationRuleRepository automationRuleRepository) : IRequestHandler<RemoveIrrigationUnitCommand, Result>
{
    public async Task<Result> Handle(RemoveIrrigationUnitCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if the field belongs to the specified farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if field has irrigation
        var unit = await irrigationUnitRepository.FindByFieldIdAsync(field.Id, cancellationToken);
        if (unit is null)
            return Result.Failure(IrrigationUnitErrors.NoUnitAssigned);
        
        // check if user can remove (only owner and manager can remove)
        if (farmUserRole.FarmRole.Name != "Owner" && farmUserRole.FarmRole.Name != "Manager")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);

        var now = DateTime.UtcNow;
        
        // get all automation rules for this farm and delete those using this irrigation unit
        var farmAutomationRules = await automationRuleRepository.FindByFarmIdAsync(request.FarmId, cancellationToken);
        var rulesToDelete = farmAutomationRules.Where(r => !r.IsDeleted && r.IrrigationUnitId == unit.Id).ToList();
        foreach (var rule in rulesToDelete)
        {
            rule.IsDeleted = true;
            rule.DeletedOn = now;
            rule.DeletedById = request.RequesterId;
            
            await automationRuleRepository.UpdateAsync(rule, cancellationToken);
        }
        
        unit.IsDeleted = true;
        unit.DeletedById = request.RequesterId;
        unit.DeletedOn = now;
        unit.UpdatedById = request.RequesterId;
        unit.UpdatedOn = now;
        unit.Device.IsAssigned = false;
        unit.Device.AssignedAt = null;
        unit.Device.UpdatedById = request.RequesterId;
        unit.Device.UpdatedOn = now;

        // soft delete associated irrigation events
        foreach (var irrigationEvent in unit.IrrigationEvents.Where(ie => !ie.IsDeleted))
        {
            irrigationEvent.IsDeleted = true;
            irrigationEvent.DeletedOn = now;
            irrigationEvent.DeletedById = request.RequesterId;
        }

        await irrigationUnitRepository.UpdateAsync(unit, cancellationToken);
        await irrigationUnitDeviceRepository.UpdateAsync(unit.Device, cancellationToken);
        
        return Result.Success();
    }
}