using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid credentials.");
    public static readonly Error UserNotFound = new("User.NotFound", "User not found.");
    public static readonly Error DuplicateEmail = new("User.DuplicateEmail", "A user with this email already exists.");
    public static readonly Error DuplicateUserName = new("User.DuplicateUserName", "A user with this username already exists.");
    public static readonly Error RegistrationFailed = new("User.RegistrationFailed", "Registration failed.");
    public static readonly Error EmailNotConfirmed = new("User.EmailNotConfirmed", "Email is not confirmed.");
    public static readonly Error InvalidEmailConfirmationToken = new("User.InvalidEmailConfirmationToken", "Invalid email confirmation token."); // the token as a whole though like the Jwt
    public static readonly Error EmailAlreadyConfirmed =
        new("User.EmailAlreadyConfirmed", "The email is already confirmed.");

    public static readonly Error EmailConfirmationFailed =
        new("User.EmailConfirmationFailed", "Email confirmation failed.");

    public static readonly Error GlobalRoleAssignmentFailed =
        new("User.GlobalRoleAssignmentFailed", "Global role assignment failed.");
    public static readonly Error ResetPasswordFailed =
        new("User.ResetPasswordFailed", "Reset password failed.");
    public static readonly Error InvalidOtp = new("User.InvalidOtp", "Invalid or expired OTP.");
    public static readonly Error InvalidPasswordResetToken = new("User.InvalidPasswordResetToken", "Invalid or expired password reset token.");
    public static readonly Error InvalidGoogleToken = new("User.InvalidGoogleToken", "Invalid Google ID token.");
    public static readonly Error MaxFarmsReached = new("User.MaxFarmsReached", "You have reached the maximum number of farms allowed for your subscription plan.");
    public static readonly Error MaxFieldsReached = new("User.MaxFieldsReached", "You have reached the maximum number of fields allowed for your subscription plan.");

    public static readonly Error MfaOtpSent = new("User.MfaOtpSent",
        "Multi-factor authentication is required. Please enter the OTP code sent to your email.");
    public static readonly Error TooManyMfaRequests = new("User.TooManyMfaRequests",
        "Too many multi-factor authentication requests. Please wait before trying again.");
    public static readonly Error CannotCancelBasicPlan = new("User.CannotCancelBasicPlan",
        "You cannot cancel the Basic plan. Please upgrade to a paid plan before cancelling your subscription.");
}