using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(IUserRepository userRepository, ILogger<RegisterCommandHandler> logger, IEmailBodyBuilder emailBodyBuilder, IOptions<AppSettings> appSettings, IEmailService emailService) : IRequestHandler<RegisterCommand, Result>
{
    public async Task<Result> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await userRepository.FindByEmailAsync(request.Email) is not null)
            return Result.Failure(UserErrors.DuplicateEmail);
        
        if (await userRepository.FindByUserNameAsync(request.UserName) is not null)
            return Result.Failure(UserErrors.DuplicateUserName);

        var user = new ApplicationUserModel
        {
            Email = request.Email,
            UserName = request.UserName,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        var success = await userRepository.CreateUserAsync(user, request.Password);

        if (!success)
            return Result.Failure(UserErrors.RegistrationFailed);

        var applicationUser = await userRepository.FindByEmailAsync(user.Email);
        if (applicationUser is null)
            return Result.Failure(UserErrors.UserNotFound); // but this is impossible but just in case like :|

        var code = await userRepository.GenerateEmailConfirmationTokenInLinkAsync(applicationUser);

        await SendConfirmationEmail(applicationUser, code);
        
        logger.LogInformation("Confirmation Code: {code}", code);
        
        return Result.Success();
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