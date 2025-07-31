using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.Notifications.Contracts;

public record NotificationResponse(
    Guid Id,
    NotificationType Type, 
    string Message,
    Guid FarmId,
    Guid? FieldId,
    DateTime CreatedOn,
    bool IsRead
); 