using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Conversation.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Handlers;

public class ClearConversationCommandHandler(IClearedConversationRepository clearedConversationRepository, IConversationMemberRepository memberRepository) : IRequestHandler<ClearConversationCommand, Result>
{
    public async Task<Result> Handle(ClearConversationCommand request, CancellationToken cancellationToken)
    {
        var isMember = await memberRepository.IsMemberAsync(request.UserId, request.ConversationId, cancellationToken);
        if (!isMember)
            return Result.Failure(ConversationErrors.AccessDenied);

        var cleared = new ClearedConversation
        {
            UserId = request.UserId,
            ConversationId = request.ConversationId,
            ClearedAt = DateTime.UtcNow
        };

        await clearedConversationRepository.AddAsync(cleared, cancellationToken);

        return Result.Success();
    }
}