using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmUserRoleErrors
{
    public static readonly Error RoleNotFound =
        new("FarmUserRole.RoleNotFound", "The specified user role does not exist in the system.");
    public static readonly Error InsufficientPermission = new(
        "FarmUserRole.InsufficientPermission",
        "You do not have the required role to perform this action on this farm.");
    public static readonly Error UserAlreadyHasAccess = new(
        "FarmUserRole.UserAlreadyHasAccess",
        "The specified user already has access to this farm.");
    public static readonly Error SelfRevokeNotAllowed = new(
        "FarmUserRole.SelfRevokeNotAllowed",
        "You cannot revoke your own access to the farm.");
}