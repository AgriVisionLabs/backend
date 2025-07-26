namespace Agrivision.Backend.Application.Services.Hubs;

public interface INotificationBroadcaster
{
    Task BroadcastNotificationAsync(string userId, object notification);
} 