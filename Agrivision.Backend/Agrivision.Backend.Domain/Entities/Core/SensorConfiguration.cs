using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class SensorConfiguration : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeviceId { get; set; }
    public SensorUnitDevice SensorUnitDevice { get; set; } = default!;
    
    public SensorType Type { get; set; }
    public string Pin { get; set; } = default!;
    
    public string? CalibrationDataJson { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<SensorReading> SensorReadings { get; set; } = new List<SensorReading>();
}