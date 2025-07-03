using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record RemoveUserFromConversationCommand(
    string RequesterId,
    Guid ConversationId,
    string TargetUserId
) : IRequest<Result>;