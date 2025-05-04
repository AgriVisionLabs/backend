using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.AutomationRules.Contracts;

public record AutomationRuleResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string FieldName,
    string Name,
    bool IsEnabled,
    AutomationRuleType Type,
    Guid SensorId,
    Guid IrrigationUnitId,
    float? MinThresholdValue,
    float? MaxThresholdValue,
    SensorType? TargetSensorType,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    DaysOfWeek? ActiveDays
);
