using Agrivision.Backend.Domain.Entities;

namespace Agrivision.Backend.Application.Auth;

public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(IApplicationUser user);
    string? ValidateToken(string token);
}