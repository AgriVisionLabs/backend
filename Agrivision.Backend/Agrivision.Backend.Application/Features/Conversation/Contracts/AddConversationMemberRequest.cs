namespace Agrivision.Backend.Application.Features.Conversation.Contracts;

public record AddConversationMemberRequest(IReadOnlyList<string> MembersList);