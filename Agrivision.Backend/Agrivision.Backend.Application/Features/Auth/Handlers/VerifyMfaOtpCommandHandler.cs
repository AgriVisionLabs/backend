using System.Security.Cryptography;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Identity;
using Agrivision.Backend.Domain.Enums.Identity;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class VerifyMfaOtpCommandHandler(IUserRepository userRepository, IOtpProvider otpProvider, IJwtProvider jwtProvider, IGlobalRoleRepository globalRoleRepository, IOptions<RefreshTokenSettings> refreshTokenSettings) : IRequestHandler<VerifyMfaOtpCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(VerifyMfaOtpCommand request, CancellationToken cancellationToken)
    {
        // find user by email
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidOtp);

        // check if the otp is valid and mark it as used 
        var isValid =
            await otpProvider.VerifyAndConsumeAsync(user.Id, request.OtpCode, OtpPurpose.MultiFactorAuth, cancellationToken);
        if (!isValid)
            return Result.Failure<AuthResponse>(UserErrors.InvalidOtp);
        
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
}