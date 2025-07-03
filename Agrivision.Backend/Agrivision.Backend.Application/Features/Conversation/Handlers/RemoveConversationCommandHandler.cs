using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Conversation.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Handlers;

public class RemoveConversationCommandHandler(IConversationRepository conversationRepository, IConversationMemberRepository conversationMemberRepository, IConversationBroadcaster conversationBroadcaster) : IRequestHandler<RemoveConversationCommand, Result>
{
    public async Task<Result> Handle(RemoveConversationCommand request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.FindByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result.Failure(ConversationErrors.NotFound);

        if (conversation.IsGroup)
        {
            var isAdmin = await conversationMemberRepository.IsAdminAsync(request.RequesterId, request.ConversationId, cancellationToken);
            if (!isAdmin)
                return Result.Failure(ConversationErrors.AccessDenied);
        }
        else
        {
            var isMember = await conversationMemberRepository.IsMemberAsync(request.RequesterId, request.ConversationId, cancellationToken);
            if (!isMember)
                return Result.Failure(ConversationErrors.AccessDenied);
        }

        var members = await conversationMemberRepository.GetMembersAsync(request.ConversationId, cancellationToken);
        var participantIds = members.Select(m => m.UserId).Distinct().ToList();

        await conversationRepository.RemoveAsync(conversation, cancellationToken);

        var broadcastTasks = participantIds.Select(userId =>
            conversationBroadcaster.BroadcastConversationRemovedAsync(userId, request.ConversationId)
        );

        await Task.WhenAll(broadcastTasks);

        return Result.Success();
    }
}