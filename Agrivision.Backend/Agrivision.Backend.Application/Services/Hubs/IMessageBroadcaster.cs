using Agrivision.Backend.Application.Features.Messages.Contracts;

namespace Agrivision.Backend.Application.Services.Hubs;

public interface IMessageBroadcaster
{
    Task BroadcastNewMessageAsync(Guid conversationId, MessageResponse response);
    Task BroadcastMessageDeletedAsync(Guid conversationId, Guid messageId);
    Task BroadcastMessageEditedAsync(Guid conversationId, MessageResponse response);
    Task BroadcastReactionAddedAsync(Guid conversationId, MessageResponse response);
    Task BroadcastReactionRemovedAsync(Guid conversationId, MessageResponse response);
}