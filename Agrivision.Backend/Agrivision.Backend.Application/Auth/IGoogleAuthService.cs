using Agrivision.Backend.Application.Models;

namespace Agrivision.Backend.Application.Auth;

public interface IGoogleAuthService
{
    Task<GoogleUserPayload?> ValidateGoogleTokenAsync(string idToken);
} 