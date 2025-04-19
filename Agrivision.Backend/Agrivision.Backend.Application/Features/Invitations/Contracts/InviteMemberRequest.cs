namespace Agrivision.Backend.Application.Features.Invitations.Contracts;

public record InviteMemberRequest
(
    string Recipient,
    int RoleId
);