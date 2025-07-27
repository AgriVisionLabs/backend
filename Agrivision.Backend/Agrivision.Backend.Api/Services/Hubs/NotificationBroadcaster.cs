using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Features.Notifications.Contracts;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Entities.Core;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class NotificationBroadcaster(IHubContext<NotificationHub> hubContext) : INotificationBroadcaster
{
    public Task BroadcastNotificationAsync(string userId, Notification notification)
    {
        var response = new NotificationResponse(
            notification.Id,
            notification.Type,
            notification.Message,
            notification.FarmId,
            notification.FieldId,
            notification.CreatedOn
        );
        
        return hubContext.Clients.Group($"user-{userId}").SendAsync("ReceiveNotification", response);
    }
} 