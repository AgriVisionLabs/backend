using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ConfirmEmailCommandHandler(IUserRepository userRepository,IJwtProvider jwtProvider) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var userId = jwtProvider.ValidateToken(request.Token, true);
        if (userId is null)
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        
        if (await userRepository.FindByIdAsync(userId) is not { } user)  
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken); // we used invalid confirmation email instead of UserNotFound so we don't just outright admit that the user doesn't exist 

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        user.EmailConfirmed = true;
        await userRepository.UpdateAsync(user);

        return Result.Success();
    }
}