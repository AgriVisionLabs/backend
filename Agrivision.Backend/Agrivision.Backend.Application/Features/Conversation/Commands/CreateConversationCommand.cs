using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record CreateConversationCommand
(
    string RequesterId,
    string? Name,
    IReadOnlyList<string> MembersList
) : IRequest<Result<ConversationResponse>>;