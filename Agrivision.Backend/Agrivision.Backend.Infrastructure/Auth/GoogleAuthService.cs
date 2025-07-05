using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Infrastructure.Settings;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Infrastructure.Auth;

public class GoogleAuthService(IOptions<GoogleAuthSettings> googleAuthSettings) : IGoogleAuthService
{
    public async Task<GoogleUserPayload?> ValidateGoogleTokenAsync(string idToken)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { googleAuthSettings.Value.ClientId }
                });

            return new GoogleUserPayload
            {
                Email = payload.Email,
                GivenName = payload.GivenName,
                FamilyName = payload.FamilyName,
                Name = payload.Name,
                Picture = payload.Picture,
                Subject = payload.Subject
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
} 