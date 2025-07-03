using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class SubscribeToConversationUpdatesCommandHandler(IConversationHubNotifier conversationHubNotifier) : IRequestHandler<SubscribeToConversationUpdatesCommand, Result>
{
    public async Task<Result> Handle(SubscribeToConversationUpdatesCommand request, CancellationToken cancellationToken)
    {
        await conversationHubNotifier.AddPrivateConnectionAsync(request.ConnectionId, request.UserId);
        
        return Result.Success();
    }
}