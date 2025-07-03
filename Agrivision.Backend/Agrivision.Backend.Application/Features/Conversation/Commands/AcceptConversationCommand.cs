using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record AcceptConversationCommand
(
    string RequesterId,
    Guid ConversationId,
    bool Accept,
    string ConnectionId
) : IRequest<Result>;