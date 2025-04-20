namespace Agrivision.Backend.Application.Features.FarmRoles.Contracts;

public record FarmRolePermissionsResponse
(
    string RoleName,
    IReadOnlyList<string> Permissions
);