using System.Security.Cryptography;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ResendConfirmationEmailCommandHandler(IUserRepository userRepository, ILogger<ResendConfirmationEmailCommand> logger, IEmailBodyBuilder emailBodyBuilder, IOptions<AppSettings> appSettings, IEmailService emailService) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var code = await userRepository.GenerateEmailConfirmationTokenInLinkAsync(user);
        
        await SendConfirmationEmail(user, code);

        logger.LogInformation("Confirmation Code: {code}", code);

        return Result.Success();
    }
    
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(128));
    }

    private async Task SendConfirmationEmail(IApplicationUser user, string code)
    {
        var encodedUserId = userRepository.EncodeUserId(user.Id);
        
        logger.LogInformation("Encoded UserId: {encodedUserId}", encodedUserId);
        var emailBody = emailBodyBuilder.GenerateEmailBody("EmailConfirmation", new Dictionary<string, string>
        {
            {"{{link}}", $"{appSettings.Value.BaseUrl}/auth/emailConfirmation?userId={encodedUserId}&code={code}"}
        });

        await emailService.SendEmailAsync(user.Email, "Agrivision: Email Confirmation", emailBody);
    }
}