namespace Agrivision.Backend.Domain.Enums.Identity;

public enum SignInStatus
{
    Success,
    InvalidCredentials,
    EmailNotConfirmed,
    LockedOut
}