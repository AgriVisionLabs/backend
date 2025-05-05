using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Handlers;

public class UpdateAutomationRuleCommandHandler(IFieldRepository fieldRepository, IAutomationRuleRepository automationRuleRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<UpdateAutomationRuleCommand, Result>
{
    public async Task<Result> Handle(UpdateAutomationRuleCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // check if user has access to the field
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // expert can't access automation rules
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if automation rule exists
        var automationRule = await automationRuleRepository.FindByIdAsync(request.AutomationRuleId, cancellationToken);
        if (automationRule is null)
            return Result.Failure(AutomationRuleErrors.AutomationRuleNotFound);
        
        // check if automation rule belongs to the field
        if (automationRule.IrrigationUnit.FieldId != request.FieldId)
            return Result.Failure(FieldErrors.UnauthorizedAction);
        
        // check if automation rule name already exists
        var existingAutomationRule = await automationRuleRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existingAutomationRule != null && existingAutomationRule.Id != request.AutomationRuleId)
            return Result.Failure(AutomationRuleErrors.DuplicateNameInFarm);
        
        // update automation rule
        automationRule.Name = request.Name;
        automationRule.IsEnabled = request.IsEnabled;
        automationRule.Type = request.Type;
        automationRule.MinimumThresholdValue = request.MinThresholdValue;
        automationRule.MaximumThresholdValue = request.MaxThresholdValue;
        automationRule.TargetSensorType = request.TargetSensorType;
        automationRule.StartTime = request.StartTime;
        automationRule.EndTime = request.EndTime;
        automationRule.ActiveDays = request.ActiveDays;
        automationRule.UpdatedOn = DateTime.UtcNow;
        automationRule.UpdatedById = request.RequesterId;
        
        // save changes
        await automationRuleRepository.UpdateAsync(automationRule, cancellationToken);
        
        return Result.Success();
    }
}