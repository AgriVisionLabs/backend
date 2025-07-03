using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Services.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Services.Hubs;

public class ConversationHubNotifier(IHubContext<ConversationHub> hubContext) : IConversationHubNotifier
{
    public async Task AddConnectionAsync(string connectionId, Guid conversationId)
    {
        await hubContext.Groups.AddToGroupAsync(connectionId, $"conversation-{conversationId}");
    }

    public async Task RemoveConnectionAsync(string connectionId, Guid conversationId)
    {
        await hubContext.Groups.RemoveFromGroupAsync(connectionId, $"conversation-{conversationId}");
    }

    public async Task AddPrivateConnectionAsync(string connectionId, string userId)
    {
        await hubContext.Groups.AddToGroupAsync(connectionId, $"user-{userId}");
    }
    
    public async Task RemovePrivateConnectionAsync(string connectionId, string userId)
    {
        await hubContext.Groups.RemoveFromGroupAsync(connectionId, $"user-{userId}");
    }
}