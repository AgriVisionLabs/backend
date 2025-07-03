using Agrivision.Backend.Application.Features.Conversation.Contracts;

namespace Agrivision.Backend.Application.Services.Hubs;

public interface IConversationBroadcaster
{
    Task BroadcastNewConversationAsync(string userId, ConversationResponse response);
    Task BroadcastConversationRemovedAsync(string userId, Guid conversationId);
    Task BroadcastConversationUpdatedAsync(string userId, ConversationResponse response);
}