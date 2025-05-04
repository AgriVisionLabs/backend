using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class SensorUnit : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public SensorUnitDevice Device { get; set; } = default!;
    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;
    public Guid FieldId { get; set; }
    public Field Field { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTime InstallationDate { get; set; } = DateTime.UtcNow;
    public UnitStatus Status { get; set; } = UnitStatus.Idle;
    public DateTime? LastMaintenance { get; set; }
    public DateTime? LasMaintenanceCompleted { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? ConfigJson { get; set; }
    public bool IsOnline { get; set; }
    public int BatteryLevel { get; set; }
    public DateTime? LastSeen { get; set; }
    public string CreatedBy { get; set; } = default!;
    public string? IpAddress { get; set; }
    public string? Notes { get; set; }

    public ICollection<SensorConfiguration> SensorConfigurations { get; set; } = new List<SensorConfiguration>();
}