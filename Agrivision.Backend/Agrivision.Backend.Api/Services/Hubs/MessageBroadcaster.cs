using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class MessageBroadcaster(IHubContext<MessageHub> hubContext) : IMessageBroadcaster
{
    public Task BroadcastNewMessageAsync(Guid conversationId, MessageResponse response)
    {
        return hubContext.Clients
            .Group($"conversation-{conversationId}")
            .SendAsync("ReceiveMessage", response);
    }

    public Task BroadcastMessageDeletedAsync(Guid conversationId, Guid messageId)
    {
        return hubContext.Clients
            .Group($"conversation-{conversationId}")
            .SendAsync("MessageDeleted", messageId);
    }

    public Task BroadcastMessageEditedAsync(Guid conversationId, MessageResponse response)
    {
        return hubContext.Clients
            .Group($"conversation-{conversationId}")
            .SendAsync("MessageEdited", response);
    }

    public Task BroadcastReactionAddedAsync(Guid conversationId, MessageResponse response)
    {
        return hubContext.Clients
            .Group($"conversation-{conversationId}")
            .SendAsync("ReactionAdded", response);
    }
    
    public Task BroadcastReactionRemovedAsync(Guid conversationId, MessageResponse response)
    {
        return hubContext.Clients
            .Group($"conversation-{conversationId}")
            .SendAsync("ReactionRemoved", response);
    }
}