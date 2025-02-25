using Agrivision.Backend.Domain.Interfaces;

namespace Agrivision.Backend.Application.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(IApplicationUser user);
    string? ValidateToken(string token);
    string GenerateEmailConfirmationJwtToken(string userId, string emailConfirmationCode);
    (string?, string?) ValidateEmailConfirmationJwtToken(string token);
}