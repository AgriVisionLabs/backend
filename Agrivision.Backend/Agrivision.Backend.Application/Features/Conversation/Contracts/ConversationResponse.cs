using Agrivision.Backend.Application.Models;

namespace Agrivision.Backend.Application.Features.Conversation.Contracts;

public record ConversationResponse
(
    Guid Id,
    string Name,
    bool IsGroup,
    string? AdminId,
    IReadOnlyList<ReceiverModel> MembersList
);