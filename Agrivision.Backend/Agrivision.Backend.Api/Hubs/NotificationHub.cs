using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;

namespace Agrivision.Backend.Api.Hubs;

[Authorize]
public class NotificationHub(IMediator mediator, INotificationHubNotifier notifier) : Hub
{
    public async Task SubscribeToNotificationUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        var connectionId = Context.ConnectionId;

        // Subscribe logic (the CQRS command already handles adding to SignalR group)
        await mediator.Send(new SubscribeToNotificationUpdatesCommand(connectionId, userId));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var connectionId = Context.ConnectionId;

        if (!string.IsNullOrWhiteSpace(userId))
        {
            await notifier.RemoveConnectionAsync(connectionId, userId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}