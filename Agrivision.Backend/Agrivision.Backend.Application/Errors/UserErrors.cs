using Agrivision.Backend.Application.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid Credentials");
    public static readonly Error UserNotFound = new("User.NotFound", "User Not Found");
}