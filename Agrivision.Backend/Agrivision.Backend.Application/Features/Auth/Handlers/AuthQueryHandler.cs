using System.Security.Cryptography;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Features.Auth.Queries;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities;
using Agrivision.Backend.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class AuthQueryHandler(IAuthRepository authRepository, IUserRepository userRepository, IJwtProvider jwtProvider, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<AuthQuery, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(AuthQuery request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

        var checkPasswordResult = await authRepository.PasswordSignInAsync(user, request.Password);

        switch (checkPasswordResult)
        {
            case SignInStatus.InvalidCredentials:
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            case SignInStatus.EmailNotConfirmed:
                return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);
            case SignInStatus.Success:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        var (token, expiresIn) = jwtProvider.GenerateToken(user);
        
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(refreshTokenSettings.Value.RefreshTokenExpiryDays);
        
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
    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
    }
}