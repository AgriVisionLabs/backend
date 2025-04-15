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
        "FarmUserRole.RoleNotFound" => StatusCodes.Status404NotFound,
        "FarmRole.RoleNotFound" => StatusCodes.Status404NotFound,
        "Invitation.AlreadyExists" => StatusCodes.Status409Conflict,
        "Invitation.SelfInvitation" => StatusCodes.Status409Conflict,
        "Invitation.CannotInviteAsOwner" => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status400BadRequest // Default case
    };
}