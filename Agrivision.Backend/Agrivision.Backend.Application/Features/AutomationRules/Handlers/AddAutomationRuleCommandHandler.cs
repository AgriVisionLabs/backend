using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.AutomationRules.Commands;
using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Handlers;

public class AddAutomationRuleCommandHandler(IFieldRepository fieldRepository, ISensorUnitRepository sensorUnitRepository, IAutomationRuleRepository automationRuleRepository, IFarmUserRoleRepository farmUserRoleRepository, IIrrigationUnitRepository irrigationUnitRepository) : IRequestHandler<AddAutomationRuleCommand, Result<AutomationRuleResponse>>
{
    public async Task<Result<AutomationRuleResponse>> Handle(AddAutomationRuleCommand request, CancellationToken cancellationToken)
    {
        // check if field exists
        var field = await fieldRepository.FindByIdAsync(request.FieldId, cancellationToken);
        if (field is null)
            return Result.Failure<AutomationRuleResponse>(FieldErrors.FieldNotFound);
        
        // check if field belongs to farm
        if (field.FarmId != request.FarmId)
            return Result.Failure<AutomationRuleResponse>(FarmErrors.UnauthorizedAction);
        
        // check if user has permission to add automation rule
        var farmUserRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<AutomationRuleResponse>(FarmErrors.UnauthorizedAction);

        if (farmUserRole.FarmRole.Name == "Worker" || farmUserRole.FarmRole.Name == "Expert")
            return Result.Failure<AutomationRuleResponse>(FarmErrors.UnauthorizedAction);
        
        // check if automation rule with same name exists
        var existingRule = await automationRuleRepository.FindByNameAndFarmIdAsync(request.Name, request.FarmId, cancellationToken);
        if (existingRule is not null)
            return Result.Failure<AutomationRuleResponse>(AutomationRuleErrors.DuplicateNameInFarm);
        
        // check if irrigation unit exists
        var irrigationUnit = await irrigationUnitRepository.FindByFieldIdAsync(request.FieldId, cancellationToken);
        if (irrigationUnit is null)
            return Result.Failure<AutomationRuleResponse>(IrrigationUnitErrors.NoUnitAssigned);
        
        // check if sensor unit exists
        var sensorUnit = await sensorUnitRepository.FindByFieldIdAsync(request.FieldId, cancellationToken);
        if (sensorUnit is null)
            return Result.Failure<AutomationRuleResponse>(SensorUnitErrors.NoUnitAssigned);
        
        // create automation rule
        var automationRule = new AutomationRule
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            FarmId = request.FarmId,
            IsEnabled = true,
            Type = request.Type,
            MinimumThresholdValue = request.MinThresholdValue,
            MaximumThresholdValue = request.MaxThresholdValue,
            TargetSensorType = request.TargetSensorType,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            ActiveDays = request.ActiveDays,
            SensorUnitId = sensorUnit.Id,
            IrrigationUnitId = irrigationUnit.Id,
            CreatedById = request.RequesterId,
            CreatedOn = DateTime.UtcNow
        };
        
        // add to db
        await automationRuleRepository.AddAsync(automationRule, cancellationToken);
        
        // map to response
        var response = new AutomationRuleResponse(automationRule.Id,
            field.FarmId,
            field.Id,
            field.Name,
            automationRule.Name,
            automationRule.IsEnabled,
            automationRule.Type,
            sensorUnit.Id,
            irrigationUnit.Id,
            automationRule.MinimumThresholdValue,
            automationRule.MaximumThresholdValue,
            automationRule.TargetSensorType,
            automationRule.StartTime,
            automationRule.EndTime,
            automationRule.ActiveDays);

        return Result.Success(response);
    }
}