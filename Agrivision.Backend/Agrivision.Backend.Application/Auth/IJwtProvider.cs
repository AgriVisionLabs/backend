using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Interfaces.Identity;

namespace Agrivision.Backend.Application.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(IApplicationUser user, IEnumerable<string> roles, IEnumerable<FarmMember> userFarmRoles);
    string? ValidateToken(string token);
    string GenerateEmailConfirmationJwtToken(string userId, string emailConfirmationCode, string email);
    (string?, string?, string?) ValidateEmailConfirmationJwtToken(string token);
}