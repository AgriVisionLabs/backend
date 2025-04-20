using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;
public static class OtpErrors
{
    public static readonly Error InvalidOtp = new("User.InvalidOtp", "Invalid or expired OTP.");
  
    public static readonly Error ExceedLimit = new("User.ExceedLimit", "Too many OTP requests. Please try again later.");
}
