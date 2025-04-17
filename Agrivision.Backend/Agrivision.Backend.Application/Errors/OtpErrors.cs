using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;
public static class OtpErrors
{
    public static readonly Error InvalidOtp = new("User.InvalidOtp", "Invalid or expired OTP.");

}
