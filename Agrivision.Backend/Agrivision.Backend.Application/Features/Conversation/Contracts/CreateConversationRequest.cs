namespace Agrivision.Backend.Application.Features.Conversation.Contracts;

public record CreateConversationRequest
(
    string? Name,
    List<string> MembersList
);