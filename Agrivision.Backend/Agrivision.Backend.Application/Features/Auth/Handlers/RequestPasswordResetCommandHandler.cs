using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Identity;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class RequestPasswordResetCommandHandler(IUserRepository userRepository, IOtpProvider otpProvider, IEmailService emailService) : IRequestHandler<RequestPasswordResetCommand, Result>
{
    public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Success(); // lie 
        
        if (!user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailNotConfirmed);

        var rawOtp = await otpProvider.GenerateAsync(user.Id, OtpPurpose.PasswordReset, cancellationToken);

        await emailService.SendPasswordResetEmailAsync(user.Email, rawOtp);

        return Result.Success();
    }
}