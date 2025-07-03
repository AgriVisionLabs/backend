using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class ConversationBroadcaster(IHubContext<ConversationHub> hubContext) : IConversationBroadcaster
{
    public Task BroadcastNewConversationAsync(string userId, ConversationResponse response)
    {
        return hubContext.Clients
            .Group($"user-{userId}")
            .SendAsync("NewConversation", response);
    }

    public Task BroadcastConversationRemovedAsync(string userId, Guid conversationId)
    {
        return hubContext.Clients
            .Group($"user-{userId}")
            .SendAsync("ConversationRemoved", conversationId);
    }

    public Task BroadcastConversationUpdatedAsync(string userId, ConversationResponse response)
    {
        return hubContext.Clients
            .Group($"user-{userId}")
            .SendAsync("ConversationUpdated", response);
    }
}