using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmUserRoleErrors
{
    public static readonly Error UserRoleNotFound =
        new("FarmUserRole.RoleNotFound", "The specified farm user role does not exist in the system.");
    public static readonly Error InsufficientPermission = new(
        "FarmUserRole.InsufficientPermission",
        "You do not have the required role to perform this action on this farm.");
    public static readonly Error UserAlreadyHasAccess = new(
        "FarmUserRole.UserAlreadyHasAccess",
        "The specified user already has access to this farm.");
    public static readonly Error SelfRevokeNotAllowed = new(
        "FarmUserRole.SelfRevokeNotAllowed",
        "You cannot revoke your own access to the farm.");
    public static readonly Error CannotModifyElevatedRoles = new(
        "FarmUserRole.CannotModifyElevatedRoles",
        "You are not allowed to modify users with elevated roles like Manager or Owner.");
    public static readonly Error SelfModificationNotAllowed = new(
        "FarmUserRole.SelfModificationNotAllowed",
        "You cannot change your own role. Nice try.");
    public static readonly Error CannotAssignElevatedRoles = new(
        "FarmUserRole.CannotAssignElevatedRoles",
        "Only owners can assign elevated roles like Manager.");
    public static readonly Error CannotAssignOwnerRole = new(
        "FarmUserRole.CannotAssignOwnerRole",
        "Assigning the 'Owner' role is not permitted.");
}