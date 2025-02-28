using System.Security.Cryptography;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ResendConfirmationEmailCommandHandler(IUserRepository userRepository, ILogger<ResendConfirmationEmailCommand> logger, IEmailService emailService, IJwtProvider jwtProvider, IUtilityService utilityService) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var confirmationCode = await userRepository.GenerateEmailConfirmationTokenAsync(user);
        
        var token = jwtProvider.GenerateEmailConfirmationJwtToken(user.Id, confirmationCode);

        var encodedToken = utilityService.Encode(token);
        
        await emailService.SendConfirmationEmail(user.Email, encodedToken);

        logger.LogInformation("Confirmation token: {token}", encodedToken);

        return Result.Success();
    }
}