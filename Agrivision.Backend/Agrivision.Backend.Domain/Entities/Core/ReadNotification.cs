using Agrivision.Backend.Domain.Entities.Shared;

namespace Agrivision.Backend.Domain.Entities.Core;

public class ReadNotification : AuditableEntity
{
    public string UserId { get; set; } = default!;
    public DateTime ReadAt { get; set; } = DateTime.UtcNow;
    public Guid NotificationId { get; set; }
    public Notification Notification { get; set; } = default!;
}