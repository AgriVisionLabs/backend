using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class IrrigationUnitDevice : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string SerialNumber { get; set; } = default!;
    public string MacAddress { get; set; } = default!;
    public string FirmwareVersion { get; set; } = default!;
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }

    public DateTime ManufacturedOn { get; set; } = DateTime.UtcNow;

    public bool IsAssigned { get; set; } = false;
    public DateTime? IsAssignedAt { get; set; }
    public string ProvisioningKey { get; set; } = default!;
}