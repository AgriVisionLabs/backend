using Agrivision.Backend.Domain.Interfaces;

namespace Agrivision.Backend.Application.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(IApplicationUser user);
    string? ValidateToken(string token, bool isEmailConfirmationToken);
    string GenerateEmailConfirmationToken(string userId);
}