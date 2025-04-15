using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmRoleErrors
{
    public static readonly Error RoleNotFound =
        new("FarmRole.RoleNotFound", "The specified role does not exist in the system.");
}