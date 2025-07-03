using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Handlers;

public class SubscribeToConversationCommandHandler(IConversationMemberRepository conversationMemberRepository, IConversationHubNotifier conversationHubNotifier, IMessageHubNotifier messageHubNotifier, IConversationConnectionTracker tracker) : IRequestHandler<SubscribeToConversationCommand, Result>
{
    public async Task<Result> Handle(SubscribeToConversationCommand request, CancellationToken cancellationToken)
    {
        // check if the user is a member of the conversation 
        var isMember =
            await conversationMemberRepository.IsMemberAsync(request.UserId, request.ConversationId, cancellationToken);
        if (!isMember)
            return Result.Failure(ConversationErrors.AccessDenied);

        await conversationHubNotifier.AddConnectionAsync(request.ConnectionId, request.ConversationId);
        await messageHubNotifier.AddConnectionAsync(request.ConnectionId, request.ConversationId);
        
        tracker.Add(request.ConnectionId, request.ConversationId);
        
        return Result.Success();
    }
}