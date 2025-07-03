namespace Agrivision.Backend.Application.Features.Conversation.Contracts;

public record ToggleAdminStatusResponse(string UserId, bool IsAdmin);