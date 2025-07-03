using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record AddConversationMembersCommand(
    string RequesterId,
    Guid ConversationId,
    IReadOnlyList<string> MembersList
) : IRequest<Result<ConversationResponse>>;