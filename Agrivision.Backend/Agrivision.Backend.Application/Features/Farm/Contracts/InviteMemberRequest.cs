namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record InviteMemberRequest
(
    string Recipient,
    int RoleId
);