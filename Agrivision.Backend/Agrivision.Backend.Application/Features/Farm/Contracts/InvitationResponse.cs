namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record InvitationResponse(
    Guid Id,
    string SenderId,
    string SenderUserName,
    string ReceiverEmail,
    string? ReceiverUserName,
    bool ReceiverExists,
    int RoleId,
    string RoleName,
    DateTime ExpiresAt,
    DateTime CreatedOn
);