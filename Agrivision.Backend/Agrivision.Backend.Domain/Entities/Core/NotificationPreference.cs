using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class NotificationPreference : AuditableEntity
{
    public string UserId { get; set; } = null!;
    public NotificationType NotificationType { get; set; }
    public bool IsEnabled { get; set; }
}