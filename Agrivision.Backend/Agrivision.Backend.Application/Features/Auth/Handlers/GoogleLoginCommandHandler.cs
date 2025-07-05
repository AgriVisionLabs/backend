using System.Security.Cryptography;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Domain.Enums.Identity;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class GoogleLoginCommandHandler(IGoogleAuthService googleAuthService, IUserRepository userRepository, IGlobalRoleRepository globalRoleRepository, IJwtProvider jwtProvider, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<GoogleLoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var payload = await googleAuthService.ValidateGoogleTokenAsync(request.IdToken);
        if (payload is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidGoogleToken);

        var email = payload.Email;
        var firstName = payload.GivenName ?? string.Empty;
        var lastName = payload.FamilyName ?? string.Empty;

        var user = await userRepository.FindByEmailAsync(email);
        if (user is null)
        {
            var newUser = new ApplicationUserModel
            {
                Email = email,
                UserName = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true,
                Status = ApplicationUserStatus.Active,
                CreatedAt = DateTime.UtcNow,
                RefreshTokens = new List<RefreshToken>()
            };

            var success = await userRepository.CreateUserAsync(newUser, GenerateRandomPassword());
            if (!success)
                return Result.Failure<AuthResponse>(UserErrors.RegistrationFailed);

            user = await userRepository.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound); // but this is impossible but just in case like :|

            await userRepository.AddToRoleAsync(user, "Member");
        }

        var userRoles = await userRepository.GetRolesAsync(user);
        var userPermissions = await globalRoleRepository.GetPermissionsAsync(userRoles, cancellationToken);
        var (token, expiresIn) = jwtProvider.GenerateToken(user, userRoles, userPermissions);

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

    private static string GenerateRandomPassword()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    }
} 