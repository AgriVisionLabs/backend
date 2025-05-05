using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Application.Features.AutomationRules.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Handlers;

public class GetAutomationRuleByIdQueryHandler(IFieldRepository fieldRepository, IAutomationRuleRepository automationRuleRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetAutomationRuleByIdQuery, Result<AutomationRuleResponse>>
{
    public async Task<Result<AutomationRuleResponse>> Handle(GetAutomationRuleByIdQuery request, CancellationToken cancellationToken)
    {
        // check if the field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<AutomationRuleResponse>(FieldErrors.FieldNotFound);
        
        // check if field belongs to the farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<AutomationRuleResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the user has access to the field
        var farmUserRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<AutomationRuleResponse>(FarmErrors.UnauthorizedAction);
        
        // expert can't access automation rules
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<AutomationRuleResponse>(FarmUserRoleErrors.InsufficientPermissions);
        
        // check if the automation rule exists
        var automationRule = await automationRuleRepository.FindByIdAsync(request.AutomationRuleId, cancellationToken);
        if (automationRule is null)
            return Result.Failure<AutomationRuleResponse>(AutomationRuleErrors.AutomationRuleNotFound);
        
        // check if the automation rule belongs to the field
        if (automationRule.SensorUnit.FieldId != request.FieldId)
            return Result.Failure<AutomationRuleResponse>(FieldErrors.UnauthorizedAction);
        
        // map to response
        var response = new AutomationRuleResponse(automationRule.Id, automationRule.FarmId, field.Id, field.Name, automationRule.Name, automationRule.IsEnabled, automationRule.Type, automationRule.SensorUnitId, automationRule.IrrigationUnitId, automationRule.MinimumThresholdValue, automationRule.MaximumThresholdValue, automationRule.TargetSensorType, automationRule.StartTime, automationRule.EndTime, automationRule.ActiveDays);
        
        return Result.Success(response);
    }
}