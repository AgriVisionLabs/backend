using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class RegisterCommandHandler(IUserRepository userRepository, ILogger<RegisterCommandHandler> logger, IEmailService emailService, IJwtProvider jwtProvider, IUtilityService utilityService) : IRequestHandler<RegisterCommand, Result>
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

        var emailConfirmationToken = await userRepository.GenerateEmailConfirmationTokenAsync(applicationUser);

        var token = jwtProvider.GenerateEmailConfirmationJwtToken(applicationUser.Id, emailConfirmationToken);

        var encodedToken = utilityService.Encode(token);

        await emailService.SendConfirmationEmail(applicationUser.Email, encodedToken);
        
        logger.LogInformation("Confirmation Token: {token}", encodedToken);
        
        return Result.Success();
    }
}