using System.Security.Cryptography;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Identity;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class RefreshTokenCommandHandler(IUserRepository userRepository, IJwtProvider jwtProvider, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = jwtProvider.ValidateToken(request.Token);
        if (userId is null)
            return Result.Failure<AuthResponse>(TokenErrors.InvalidToken);

        var user = await userRepository.FindByIdAsync(userId);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var userRefreshToken =
            user.RefreshTokens.SingleOrDefault(token => token.Token == request.RefreshToken && token.IsActive);
        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(TokenErrors.InvalidToken);

        // revoke now since we will refresh the both the refresh token and jwt token
        userRefreshToken.RevokedOn = DateTime.UtcNow;

        // generate new jwt token for the user
        var (newToken, expiresIn) = jwtProvider.GenerateToken(user);

        // generate a new refresh token using the private method here (the one with the random number generator)
        var newRefreshToken = GenerateRefreshToken(); 
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenSettings.Value.RefreshTokenExpiryDays);

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
    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
    }
}