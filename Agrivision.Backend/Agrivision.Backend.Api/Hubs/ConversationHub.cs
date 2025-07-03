using System.Security.Claims;
using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Hubs;

[Authorize]
public class ConversationHub(IMediator mediator, IConversationConnectionTracker tracker) : Hub
{
    public async Task SubscribeToConversationUpdates()
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        var connectionId = Context.ConnectionId;

        await mediator.Send(new SubscribeToConversationUpdatesCommand(connectionId, userId));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var conversationIds = tracker.GetConversations(connectionId);

        foreach (var convoId in conversationIds)
        {
            await mediator.Send(new UnsubscribeFromConversationCommand(connectionId, convoId));
        }

        tracker.Clear(connectionId);
        
        await base.OnDisconnectedAsync(exception);
    }
}