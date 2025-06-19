using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Handlers;

public class RemoveAutomationRuleCommandHandler(IFieldRepository fieldRepository, IAutomationRuleRepository automationRuleRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<RemoveAutomationRuleCommand, Result>
{
    public async Task<Result> Handle(RemoveAutomationRuleCommand request, CancellationToken cancellationToken)
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
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // expert can't access automation rules
        if (farmUserRole.FarmRole.Name != "Owner" && farmUserRole.FarmRole.Name != "Manager")
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if automation rule exists
        var automationRule = await automationRuleRepository.FindByIdAsync(request.AutomationRuleId, cancellationToken);
        if (automationRule is null)
            return Result.Failure(AutomationRuleErrors.AutomationRuleNotFound);
        
        // check if automation rule belongs to the field
        if (automationRule.IrrigationUnit.FieldId != request.FieldId)
            return Result.Failure(FieldErrors.UnauthorizedAction);

        automationRule.IsDeleted = true;
        automationRule.DeletedOn = DateTime.UtcNow;
        automationRule.DeletedById = request.RequesterId;
        
        // save changes
        await automationRuleRepository.UpdateAsync(automationRule, cancellationToken);
        
        return Result.Success();
    }
}