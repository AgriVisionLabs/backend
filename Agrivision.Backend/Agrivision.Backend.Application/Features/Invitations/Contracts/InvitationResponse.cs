namespace Agrivision.Backend.Application.Features.Invitations.Contracts;

public record InvitationResponse(
    Guid Id,
    Guid FarmId,
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