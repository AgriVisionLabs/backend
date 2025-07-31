using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Services.Hubs;

public interface INotificationBroadcaster
{
    Task BroadcastNotificationAsync(string userId, Notification notification, bool isRead = false);
} 