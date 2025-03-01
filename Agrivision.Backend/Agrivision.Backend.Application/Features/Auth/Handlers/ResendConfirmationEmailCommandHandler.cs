using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ResendConfirmationEmailCommandHandler(IUserRepository userRepository, ILogger<ResendConfirmationEmailCommand> logger, IEmailService emailService, IJwtProvider jwtProvider) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var confirmationCode = await userRepository.GenerateEmailConfirmationTokenAsync(user);
        
        var token = jwtProvider.GenerateEmailConfirmationJwtToken(user.Id, confirmationCode, user.Email);
        
        await emailService.SendConfirmationEmail(user.Email, token);

        logger.LogInformation("Confirmation token: {token}", token);

        return Result.Success();
    }
}