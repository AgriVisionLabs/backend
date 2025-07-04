using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class AutomationRule : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;

    public bool IsActive { get; set; }
    public bool IsEnabled { get; set; }

    public AutomationRuleType Type { get; set; } // enum: Threshold, Scheduled

    public Guid? SensorUnitId { get; set; }
    public SensorUnit? SensorUnit { get; set; }

    public Guid IrrigationUnitId { get; set; }
    public IrrigationUnit IrrigationUnit { get; set; } = null!;

    // threshold
    public SensorType? TargetSensorType { get; set; } // e.g., moisture
    public float? MinimumThresholdValue { get; set; }
    public float? MaximumThresholdValue { get; set; } // for range checks

    // scheduled
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }

    public DaysOfWeek? ActiveDays { get; set; } // e.g., flags enum if you want Mon–Fri only
}