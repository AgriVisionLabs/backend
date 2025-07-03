namespace Agrivision.Backend.Application.Features.Conversation.Contracts;

public record AcceptConversationRequest(bool Accept, string ConnectionId);