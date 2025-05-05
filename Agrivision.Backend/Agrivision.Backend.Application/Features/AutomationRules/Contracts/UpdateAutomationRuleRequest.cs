using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.AutomationRules.Contracts;

public record UpdateAutomationRuleRequest
(
    string Name,
    bool IsEnabled,
    AutomationRuleType Type,
    float? MinThresholdValue,
    float? MaxThresholdValue,
    SensorType? TargetSensorType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DaysOfWeek? ActiveDays
);