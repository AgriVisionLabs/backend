namespace Agrivision.Backend.Application.Features.Farm.Contracts;

public record FarmMemberResponse
(
    string MemberId,
    Guid FarmId,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    string RoleName,
    DateTime JoinedAt,
    string InvitedByUserName,
    string InvitedById,
    bool IsOwner
);