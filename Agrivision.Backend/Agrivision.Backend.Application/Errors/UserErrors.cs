using Agrivision.Backend.Application.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid credentials.");
    public static readonly Error UserNotFound = new("User.NotFound", "User not found.");
    public static readonly Error DuplicateEmail = new("User.DuplicateEmail", "A user with this email already exists.");
    public static readonly Error RegistrationFailed = new("User.RegistrationFailed", "Registration failed.");
}