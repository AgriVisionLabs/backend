using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Messages.Commands;
using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Messages.Handlers;

public class SendMessageCommandHandler(IMessageRepository messageRepository, IConversationMemberRepository memberRepository, IMessageBroadcaster broadcaster)
    : IRequestHandler<SendMessageCommand, Result<MessageResponse>>
{
    public async Task<Result<MessageResponse>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var isMember = await memberRepository.IsMemberAsync(request.SenderId, request.ConversationId, cancellationToken);
        if (!isMember)
            return Result.Failure<MessageResponse>(ConversationErrors.AccessDenied);

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            SenderId = request.SenderId,
            Content = request.Content,
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.SenderId
        };

        await messageRepository.AddAsync(message, cancellationToken);

        var response = new MessageResponse(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Content,
            message.CreatedOn
        );

        await broadcaster.BroadcastNewMessageAsync(request.ConversationId, response);

        return Result.Success(response);
    }
}