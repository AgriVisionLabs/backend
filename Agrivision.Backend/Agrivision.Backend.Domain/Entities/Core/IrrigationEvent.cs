using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class IrrigationEvent : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid IrrigationUnitId { get; set; }
    public IrrigationUnit IrrigationUnit { get; set; } = default!;
    
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    public IrrigationTriggerMethod TriggerMethod { get; set; } = IrrigationTriggerMethod.Manual;
}