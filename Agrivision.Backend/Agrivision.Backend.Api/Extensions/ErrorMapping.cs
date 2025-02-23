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
        "Token.InvalidToken" => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest // Default case
    };
}