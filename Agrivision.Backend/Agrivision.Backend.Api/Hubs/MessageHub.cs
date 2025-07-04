using System.Security.Claims;
using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Hubs;

[Authorize]
public class MessageHub(IMediator mediator, IConversationConnectionTracker tracker) : Hub
{
    public async Task SubscribeToConversations(Guid conversationId)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userId)) return;

        var connectionId = Context.ConnectionId;

        var result = await mediator.Send(new SubscribeToConversationCommand(conversationId, connectionId, userId));
        if (!result.Succeeded)
        {
            throw new HubException(result.Error.ToString());
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        var conversationIds = tracker.GetConversations(connectionId);

        foreach (var convoId in conversationIds)
        {
            await Groups.RemoveFromGroupAsync(connectionId, $"conversation-{convoId}");
        }

        tracker.Clear(connectionId);
        await base.OnDisconnectedAsync(exception);
    }
}