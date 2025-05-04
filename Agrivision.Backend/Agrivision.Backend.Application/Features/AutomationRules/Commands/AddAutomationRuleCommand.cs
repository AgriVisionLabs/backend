using Agrivision.Backend.Application.Features.AutomationRules.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Commands;

public record AddAutomationRuleCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string Name,
    AutomationRuleType Type,
    float? MinThresholdValue,
    float? MaxThresholdValue,
    SensorType? TargetSensorType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DaysOfWeek? ActiveDays
) : IRequest<Result<AutomationRuleResponse>>;
