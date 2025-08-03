using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Identity;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class VerifyPasswordResetOtpCommandHandler(IUserRepository userRepository, IOtpProvider otpProvider, IJwtProvider jwtProvider) : IRequestHandler<VerifyPasswordResetOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(VerifyPasswordResetOtpCommand request, CancellationToken cancellationToken)
    {
        // find user by email
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<string>(UserErrors.InvalidOtp);

        // check if the otp is valid and mark it as used 
        var isValid =
            await otpProvider.VerifyAndConsumeAsync(user.Id, request.OtpCode, OtpPurpose.PasswordReset, cancellationToken);
        if (!isValid)
            return Result.Failure<string>(UserErrors.InvalidOtp);
        
        // generate identity token
        var identityToken = await userRepository.GeneratePasswordResetTokenAsync(user);

        // wrap it in your beautiful, handcrafted artisan JWT
        var jwt = jwtProvider.GeneratePasswordResetJwt(user.Id, user.Email, identityToken);
        
        return Result.Success(jwt);
    }
}