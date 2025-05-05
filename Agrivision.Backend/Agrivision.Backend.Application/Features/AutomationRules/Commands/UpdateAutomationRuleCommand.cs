using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.AutomationRules.Commands;

public record UpdateAutomationRuleCommand
(
    Guid FarmId,
    Guid FieldId,
    Guid AutomationRuleId,
    string RequesterId,
    string Name,
    bool IsEnabled,
    AutomationRuleType Type,
    float? MinThresholdValue,
    float? MaxThresholdValue,
    SensorType? TargetSensorType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DaysOfWeek? ActiveDays
) : IRequest<Result>;