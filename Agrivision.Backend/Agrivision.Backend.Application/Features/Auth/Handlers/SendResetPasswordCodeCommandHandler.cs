
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;
public class SendResetPasswordCodeCommandHandler(IUserRepository userRepository
                                               , IOtpProvider otpProvider
                                               , IEmailService emailService
                                               , ILogger<SendResetPasswordCodeCommandHandler> logger)
                                                                                : IRequestHandler<SendResetPasswordCodeCommand, Result>
{
    public async Task<Result> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not { } applicationUser)
            return Result.Success();

        var otp =  otpProvider.GenerateOtp() ;
        await otpProvider.StoreOtpAsync(request.Email, otp, cancellationToken);

        await emailService.SendPasswordResetEmailAsync(applicationUser.Email, otp,cancellationToken);

        logger.LogInformation("OTP : {otp}", otp);

        return Result.Success();

    }
}
