namespace Agrivision.Backend.Domain.Entities.Core;

public class SensorReading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SensorConfigurationId { get; set; }
    public SensorConfiguration SensorConfiguration { get; set; } = default!;

    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public float Value { get; set; }
    public string Unit { get; set; } = default!;
}