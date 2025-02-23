using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ConfirmEmailCommandHandler(IUserRepository userRepository, ILogger<ConfirmEmailCommandHandler> logger) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (!userRepository.TryDecodeUserId(request.UserId, out var decodedUserId))
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);
        
        if (await userRepository.FindByIdAsync(decodedUserId) is not { } user)  
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode); // we used invalid confirmation email instead of UserNotFound so we don't just outright admit that the user doesn't exist 

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);
        
        if (!userRepository.TryDecodeConfirmationToken(request.Code, out var decodedCode))
            return Result.Failure(UserErrors.InvalidEmailConfirmationCode);

        var result = await userRepository.ConfirmEmailAsync(user, decodedCode);

        return result ? Result.Success() : Result.Failure(UserErrors.EmailConfirmationFailed);
    }
}