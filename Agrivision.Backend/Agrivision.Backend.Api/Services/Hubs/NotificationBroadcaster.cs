using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class NotificationBroadcaster(IHubContext<NotificationHub> hubContext) : INotificationBroadcaster
{
    public Task BroadcastNotificationAsync(string userId, object notification)
        => hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", notification);
} 