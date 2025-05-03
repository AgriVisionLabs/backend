using Agrivision.Backend.Domain.Enums.Identity;
using Agrivision.Backend.Domain.Interfaces.Identity;

namespace Agrivision.Backend.Application.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(IApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? ValidateToken(string token);
    string GenerateEmailConfirmationJwtToken(string userId, string emailConfirmationCode, string email);
    (string?, string?, string?) ValidateEmailConfirmationJwtToken(string token);
    public string GenerateOtpJwtToken(string userId, string email, OtpPurpose purpose);
    public (string? userId, string? email, OtpPurpose? purpose) ValidateOtpJwtToken(string token);
    string GeneratePasswordResetJwt(string userId, string email, string identityToken);
    (string? userId, string? email, string? identityToken) ValidatePasswordResetJwt(string token);
}