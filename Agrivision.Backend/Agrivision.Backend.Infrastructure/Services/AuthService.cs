using System.Security.Cryptography;
using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Infrastructure.Auth;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly int _refreshTokenExpiryDays = 14;
    public async Task<AuthResponse?> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
            return null;

        var isValidPassword = await userManager.CheckPasswordAsync(user, password);
        if (!isValidPassword)
            return null;

        var (token, expiresIn) = jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });
        
        await userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn,
            refreshToken, refreshTokenExpiration);
    }

    public async Task<AuthResponse?> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);
        if (userId is null)
            return null;

        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return null;

        var userRefreshToken =
            user.RefreshTokens.SingleOrDefault(token => token.Token == refreshToken && token.IsActive);
        if (userRefreshToken is null)
            return null;

        // revoke now since we will refresh the both the refresh token and jwt token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        // generate new jwt token for the user
        var (newToken, expiresIn) = jwtProvider.GenerateToken(user);

        // generate a new refresh token using the private method here (the one with the random number generator)
        var newRefreshToken = GenerateRefreshToken(); 
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        // add the new refresh token to the refresh tokens table in the database
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await userManager.UpdateAsync(user);

        return new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn,
            newRefreshToken, refreshTokenExpiration);
    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
    }
}