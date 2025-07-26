using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class NotificationHubNotifier(IHubContext<NotificationHub> hubContext) : INotificationHubNotifier
{
    public Task AddConnectionAsync(string connectionId, string userId)
        => hubContext.Groups.AddToGroupAsync(connectionId, $"user-{userId}");

    public Task RemoveConnectionAsync(string connectionId, string userId)
        => hubContext.Groups.RemoveFromGroupAsync(connectionId, $"user-{userId}");
} 