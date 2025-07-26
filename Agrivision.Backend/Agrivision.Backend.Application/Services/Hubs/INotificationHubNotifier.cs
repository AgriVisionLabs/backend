namespace Agrivision.Backend.Application.Services.Hubs;

public interface INotificationHubNotifier
{
    Task AddConnectionAsync(string connectionId, string userId);
    Task RemoveConnectionAsync(string connectionId, string userId);
} 