namespace Agrivision.Backend.Application.Features.Auth.Contracts;

public record VerifyMfaOtpRequest(string Email, string OtpCode);