using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Application.Features.AutomationRules.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Handlers;

public class GetAutomationRulesByFarmIdQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IAutomationRuleRepository automationRuleRepository) : IRequestHandler<GetAutomationRulesByFarmIdQuery, Result<IReadOnlyList<AutomationRuleResponse>>>
{
    public async Task<Result<IReadOnlyList<AutomationRuleResponse>>> Handle(GetAutomationRulesByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<AutomationRuleResponse>>(FarmErrors.FarmNotFound);
        
        // check if user has access 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<IReadOnlyList<AutomationRuleResponse>>(FarmErrors.UnauthorizedAction);
        
        // expert can't access 
        if (farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<IReadOnlyList<AutomationRuleResponse>>(FarmUserRoleErrors.InsufficientPermissions);
        
        // get the automation rules
        var automationRules =
            await automationRuleRepository.FindByFarmIdAsync(request.FarmId, cancellationToken);
        
        // map to response
        var automationRuleResponses = automationRules.Select(x => new AutomationRuleResponse
        (
            x.Id,
            x.FarmId,
            x.IrrigationUnit.FieldId,
            x.IrrigationUnit.Field.Name,
            x.Name,
            x.IsEnabled,
            x.Type,
            x.SensorUnitId,
            x.IrrigationUnitId,
            x.MinimumThresholdValue,
            x.MaximumThresholdValue,
            x.TargetSensorType,
            x.StartTime,
            x.EndTime,
            x.ActiveDays
        )).ToList();
        
        return Result.Success<IReadOnlyList<AutomationRuleResponse>>(automationRuleResponses);
    }
}