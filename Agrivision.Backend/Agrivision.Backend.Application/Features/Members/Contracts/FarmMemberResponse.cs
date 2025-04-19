namespace Agrivision.Backend.Application.Features.Members.Contracts;

public record FarmMemberResponse
(
    string MemberId,
    Guid FarmId,
    string UserName,
    string Email,
    string FirstName,
    string LastName,
    int RoleId,
    string RoleName,
    DateTime JoinedAt,
    string InvitedByUserName,
    string InvitedById,
    bool IsOwner
);