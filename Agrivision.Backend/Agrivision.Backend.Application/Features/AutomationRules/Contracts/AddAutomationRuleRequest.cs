using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.AutomationRules.Contracts;

public record AddAutomationRuleRequest
(
    string Name,
    AutomationRuleType Type,
    float? MinThresholdValue,
    float? MaxThresholdValue,
    SensorType? TargetSensorType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DaysOfWeek? ActiveDays
);