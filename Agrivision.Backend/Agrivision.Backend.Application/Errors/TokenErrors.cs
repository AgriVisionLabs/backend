using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class TokenErrors
{
    public static readonly Error InvalidToken =
        new("Token.Invalid", "The token provided is invalid or malformed.");
}