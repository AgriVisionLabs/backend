using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class IrrigationUnit : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid DeviceId { get; set; }
    public IrrigationUnitDevice Device { get; set; } = default!;

    public Guid FarmId { get; set; }
    public Farm Farm { get; set; } = default!;
    public Guid FieldId { get; set; }
    public Field Field { get; set; } = default!;

    public string Name { get; set; } = default!;
    public DateTime InstallationDate { get; set; } = DateTime.UtcNow;
    public UnitStatus Status { get; set; } = UnitStatus.Active;
    public DateTime? LastActivation { get; set; }
    public DateTime? LastDeactivation { get; set; }
    public DateTime? LastMaintenance { get; set; }
    public DateTime? LastMaintenanceCompleted { get; set; }
    public DateTime? NextMaintenance { get; set; }
    public string? ConfigJson { get; set; }
    public bool IsOnline { get; set; } = false;
    public DateTime? LastSeen { get; set; }
    public bool IsOn { get; set; }
    public string? ToggledById { get; set; }
    public string CreatedBy { get; set; }

    public string? IpAddress { get; set; }
    public string? Notes { get; set; }

    public ICollection<IrrigationEvent> IrrigationEvents { get; set; } = new List<IrrigationEvent>();
    
}