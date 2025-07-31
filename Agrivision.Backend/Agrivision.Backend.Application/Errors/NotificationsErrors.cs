using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class NotificationsErrors
{
    // not found
    public static readonly Error NotificationNotFound = new("Notifications.NotificationNotFound",
        "The specified notification does not exist in the system.");
}