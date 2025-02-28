using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ConfirmEmailCommandHandler(IUserRepository userRepository,IJwtProvider jwtProvider, IUtilityService utilityService) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var decodingResult = utilityService.TryDecode(request.Token, out var decodedToken);
        if (!decodingResult)
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        
        var (userId, confirmationCode) = jwtProvider.ValidateEmailConfirmationJwtToken(decodedToken!);
        if (userId is null || confirmationCode is null)
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        
        if (await userRepository.FindByIdAsync(userId) is not { } user)  
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken); // we used invalid confirmation email instead of UserNotFound so we don't just outright admit that the user doesn't exist 

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var result = await userRepository.ConfirmEmailAsync(user, confirmationCode);

        return result ? Result.Success() : Result.Failure(UserErrors.EmailConfirmationFailed);
    }
}