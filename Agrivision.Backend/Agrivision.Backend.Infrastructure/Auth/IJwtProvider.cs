using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;

namespace Agrivision.Backend.Infrastructure.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser user);
    string? ValidateToken(string token);
}