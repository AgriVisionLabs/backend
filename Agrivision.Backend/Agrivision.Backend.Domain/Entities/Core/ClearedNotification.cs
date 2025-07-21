using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class ClearedNotification : AuditableEntity
{
    public string UserId { get; set; } = default!;
    public DateTime ClearedAt { get; set; } = DateTime.UtcNow;
}