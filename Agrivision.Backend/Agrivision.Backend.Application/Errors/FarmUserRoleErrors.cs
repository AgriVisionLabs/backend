using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmUserRoleErrors
{
    public static readonly Error RoleNotFound =
        new("FarmUserRole.RoleNotFound", "The specified role does not exist in the system.");
}