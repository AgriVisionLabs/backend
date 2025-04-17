

namespace Agrivision.Backend.Application.Features.Auth.Contracts;
public record ResetPasswordRequest
(
   string Email,
   string Otp,
   string NewPassword
);
