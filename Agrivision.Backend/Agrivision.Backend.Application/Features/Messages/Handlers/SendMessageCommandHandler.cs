using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Messages.Commands;
using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Messages.Handlers;

public class SendMessageCommandHandler(IMessageRepository messageRepository, IConversationMemberRepository memberRepository, IMessageBroadcaster broadcaster, IConversationMemberRepository conversationMemberRepository, IConversationRepository conversationRepository, INotificationRepository notificationRepository, INotificationPreferenceRepository notificationPreferenceRepository, INotificationBroadcaster notificationBroadcaster)
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
        
        // send notification
        // conversation members
        var conversationMembers = await conversationMemberRepository.GetMembersAsync(request.ConversationId, cancellationToken);
        var conversation = await conversationRepository.FindByIdAsync(request.ConversationId, cancellationToken);
        
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.Message,
            Message = "New message in conversation " + conversation?.Name,
            CreatedById = request.SenderId,
            CreatedOn = DateTime.UtcNow,
            UserIds = conversationMembers.Select(cm => cm.UserId).ToList()
        };

        foreach (var member in conversationMembers)
        {
            var shouldNotify = await notificationPreferenceRepository.ShouldNotifyAsync(member.UserId, NotificationType.Alert, cancellationToken);
            if (shouldNotify)
                await notificationBroadcaster.BroadcastNotificationAsync(member.UserId, notification);
        }
        
        await notificationRepository.AddAsync(notification, cancellationToken);

        return Result.Success(response);
    }
}