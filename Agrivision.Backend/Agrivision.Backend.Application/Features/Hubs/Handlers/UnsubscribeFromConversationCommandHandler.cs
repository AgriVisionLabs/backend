using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class UnsubscribeFromConversationCommandHandler(IConversationHubNotifier conversationHubNotifier, IMessageHubNotifier messageHubNotifier) : IRequestHandler<UnsubscribeFromConversationCommand, Result>
{
    public async Task<Result> Handle(UnsubscribeFromConversationCommand request, CancellationToken cancellationToken)
    {
        await conversationHubNotifier.RemoveConnectionAsync(request.ConnectionId, request.ConversationId);
        await messageHubNotifier.RemoveConnectionAsync(request.ConnectionId, request.ConversationId);

        return Result.Success();
    }
}