using System.Security.Cryptography;
using Agrivision.Backend.Application.Abstractions;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Entities;

namespace Agrivision.Backend.Application.Services.Auth;

public class AuthService(IUserRepository userRepository, IJwtProvider jwtProvider) : IAuthService
{
    private readonly int _refreshTokenExpiryDays = 14;
    public async Task<Result<AuthResponse>> GetTokenAsync(AuthRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var isValidPassword = await userRepository.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var (token, expiresIn) = jwtProvider.GenerateToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        
        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });
        
        await userRepository.UpdateAsync(user);
        
        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn,
            refreshToken, refreshTokenExpiration);

        return Result.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var userId = jwtProvider.ValidateToken(token);
        if (userId is null)
            return Result.Failure<AuthResponse>(TokenErrors.InvalidAuthentication);

        var user = await userRepository.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var userRefreshToken =
            user.RefreshTokens.SingleOrDefault(token => token.Token == refreshToken && token.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(TokenErrors.InvalidAuthentication);

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

        await userRepository.UpdateAsync(user);
        
        var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn,
            newRefreshToken, refreshTokenExpiration);

        return Result.Success(authResponse);
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not null)
            return Result.Failure<AuthResponse>(UserErrors.DuplicateEmail);

        var user = new ApplicationUserModel
        {
            Email = request.Email,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var success = await userRepository.CreateUserAsync(user, request.Password);

        if (!success)
            return Result.Failure<AuthResponse>(UserErrors.RegistrationFailed);

        var applicationUser = await userRepository.FindByEmailAsync(user.Email);
        if (applicationUser is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound); // but this is impossible but just in case like :|

        var (token, expiresIn) = jwtProvider.GenerateToken(applicationUser);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);
        
        applicationUser.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });
        
        await userRepository.UpdateAsync(applicationUser);
        
        var authResponse = new AuthResponse(applicationUser.Id, applicationUser.Email, applicationUser.FirstName, applicationUser.LastName, token, expiresIn,
            refreshToken, refreshTokenExpiration);

        return Result.Success(authResponse);
    }


    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
    }
}