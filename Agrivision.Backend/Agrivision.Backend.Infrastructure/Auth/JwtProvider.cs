using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Agrivision.Backend.Infrastructure.Auth;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    public (string token, int expiresIn) GenerateToken(IApplicationUser user)
    {
        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpiryMinutes),
            signingCredentials: signingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), jwtOptions.Value.ExpiryMinutes);
    }

    public string? ValidateToken(string token, bool isEmailConfirmationToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        SymmetricSecurityKey symmetricSecurityKey;

        symmetricSecurityKey = !isEmailConfirmationToken ? new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key)) : new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.ConfirmationEmailKey));

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = symmetricSecurityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            return jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {
            return null;
        }
    }
    
    public string GenerateEmailConfirmationToken(string userId)
    {
        Claim[] claims =
        [
            new (JwtRegisteredClaimNames.Sub, userId), // Store User ID
            new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token ID
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.ConfirmationEmailKey));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken
        (
            issuer: jwtOptions.Value.Issuer,
            audience: jwtOptions.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.ConfirmationEmailExpiryMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}