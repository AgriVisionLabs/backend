using Agrivision.Backend.Application.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class TokenErrors
{
    public static readonly Error InvalidAuthentication = new("Token.InvalidAuthentication", "Invalid authentication credentials.");
}