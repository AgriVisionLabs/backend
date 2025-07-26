using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Agrivision.Backend.Application.Features.Hubs.Commands;

namespace Agrivision.Backend.Api.Hubs;

[Authorize]
public class NotificationHub(IMediator mediator) : Hub
{
    public async Task SubscribeToNotificationUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        var connectionId = Context.ConnectionId;
        await mediator.Send(new SubscribeToNotificationUpdatesCommand(connectionId, userId));
    }
}