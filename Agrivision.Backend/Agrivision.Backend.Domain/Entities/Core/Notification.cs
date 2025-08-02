using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class Notification : AuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public NotificationType Type { get; set; } = NotificationType.Alert;
    public string Message { get; set; } = default!;
    
    public Guid? FarmId { get; set; }
    public Farm? Farm { get; set; } = default!;
    
    public Guid? FieldId { get; set; }
    public Field? Field { get; set; } = default!;
    
    public ICollection<ReadNotification> ReadNotifications { get; set; } = new List<ReadNotification>();
    public ICollection<string> UserIds { get; set; } = new List<string>();
}