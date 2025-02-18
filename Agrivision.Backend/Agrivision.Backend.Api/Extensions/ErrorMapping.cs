using Agrivision.Backend.Application.Abstractions;

namespace Agrivision.Backend.Api.Extensions;

public static class ErrorMapping
{
    public static int ToStatusCode(this Error error) => error.Code switch
    {
        "User.InvalidCredentials" => StatusCodes.Status401Unauthorized,
        "User.NotFound" => StatusCodes.Status404NotFound,
        "User.DuplicateEmail" => StatusCodes.Status409Conflict,
        "Token.InvalidToken" => StatusCodes.Status401Unauthorized,
        _ => StatusCodes.Status400BadRequest // Default case
    };
}