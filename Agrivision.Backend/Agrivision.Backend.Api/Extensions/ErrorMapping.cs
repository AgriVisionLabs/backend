using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Api.Extensions;

public static class ErrorMapping
{
    public static int ToStatusCode(this Error error) => error.Code switch // we do this because we can't add individual status codes for each specific errors since it uses mvc package and errors are in the application layer
    {
        "User.InvalidCredentials" => StatusCodes.Status401Unauthorized,
        "User.NotFound" => StatusCodes.Status404NotFound,
        "User.DuplicateEmail" => StatusCodes.Status409Conflict,
        "User.DuplicateUserName" => StatusCodes.Status409Conflict,
        "User.EmailNotConfirmed" => StatusCodes.Status403Forbidden,
        "User.InvalidEmailConfirmationCode" => StatusCodes.Status403Forbidden,
        "User.EmailAlreadyConfirmed" => StatusCodes.Status400BadRequest,
        "Token.Invalid" => StatusCodes.Status401Unauthorized,
        "Farm.DuplicateName" => StatusCodes.Status409Conflict,
        "Farm.NotFound" => StatusCodes.Status404NotFound,
        "Farm.InvalidFarmId" => StatusCodes.Status400BadRequest,
        "Farm.UnauthorizedAction" => StatusCodes.Status403Forbidden,
        "Field.DuplicateName" => StatusCodes.Status409Conflict,
        "Field.UnauthorizedAction" => StatusCodes.Status403Forbidden,
        "Field.NotFound" => StatusCodes.Status404NotFound,
        "Field.InvalidArea" => StatusCodes.Status400BadRequest,
        "User.GlobalRoleAssignmentFailed" => StatusCodes.Status422UnprocessableEntity,
        "FarmUserRole.UserRoleNotFound" => StatusCodes.Status404NotFound,
        "FarmRole.RoleNotFound" => StatusCodes.Status404NotFound,
        "FarmInvitation.AlreadyExists" => StatusCodes.Status409Conflict,
        "FarmInvitation.SelfInvitation" => StatusCodes.Status409Conflict,
        "FarmInvitation.CannotInviteAsOwner" => StatusCodes.Status409Conflict,
        "FarmUserRole.InsufficientPermission" => StatusCodes.Status403Forbidden,
        "FarmInvitation.InvalidToken" => StatusCodes.Status403Forbidden,
        "FarmUserRole.UserAlreadyHasAccess" => StatusCodes.Status409Conflict,
        "FarmInvitation.InvitationNotFound" => StatusCodes.Status404NotFound,
        "FarmInvitation.UnauthorizedAction" => StatusCodes.Status401Unauthorized,
        "FarmInvitation.AlreadyAccepted" => StatusCodes.Status409Conflict,
        "FarmUserRole.SelfRevokeNotAllowed" => StatusCodes.Status403Forbidden,
        "FarmUserRole.CannotModifyElevatedRoles" => StatusCodes.Status403Forbidden,
        "FarmUserRole.SelfModificationNotAllowed" => StatusCodes.Status400BadRequest,
        "FarmUserRole.CannotAssignElevatedRoles" => StatusCodes.Status403Forbidden,
        "FarmUserRole.CannotAssignOwnerRole" => StatusCodes.Status403Forbidden,
        "IrrigationUnit.NoUnitAssigned" => StatusCodes.Status404NotFound,
        "IrrigationUnit.DeviceOffline" => StatusCodes.Status400BadRequest,
        "IrrigationUnit.FailedToSendCommand" => StatusCodes.Status503ServiceUnavailable,
        "IrrigationDeviceUnit.NotFound" => StatusCodes.Status404NotFound,
        "IrrigationUnit.DeviceUnreachable" => StatusCodes.Status503ServiceUnavailable,
        "Field.AlreadyHasIrrigationUnit" => StatusCodes.Status409Conflict,
        "IrrigationUnit.DuplicateNameInFarm" => StatusCodes.Status409Conflict,
        "IrrigationDeviceUnit.AlreadyAssigned" => StatusCodes.Status409Conflict,
        "User.ResetPasswordFailed" => StatusCodes.Status400BadRequest,
        "User.InvalidPasswordResetOtp" => StatusCodes.Status401Unauthorized,
        "User.InvalidPasswordResetToken" => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest // Default case
    };
}