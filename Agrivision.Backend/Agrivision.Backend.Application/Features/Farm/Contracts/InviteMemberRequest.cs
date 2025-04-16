namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record InviteMemberRequest
(
    Guid FarmId,
    string Recipient,
    int RoleId
);