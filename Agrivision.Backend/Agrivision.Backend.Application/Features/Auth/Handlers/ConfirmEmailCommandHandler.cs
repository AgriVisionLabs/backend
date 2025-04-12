using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;

public class ConfirmEmailCommandHandler(IUserRepository userRepository,IJwtProvider jwtProvider) : IRequestHandler<ConfirmEmailCommand, Result>
{
    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var (userId, confirmationCode, email) = jwtProvider.ValidateEmailConfirmationJwtToken(request.Token);
        if (userId is null || confirmationCode is null || email is null)
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        
        var userById = await userRepository.FindByIdAsync(userId);
        var userByEmail = await userRepository.FindByEmailAsync(email);
        
        if (userById is null || userByEmail is null || userById != userByEmail)
            return Result.Failure(UserErrors.InvalidEmailConfirmationToken);
        
        var user = userById;
      
        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.EmailAlreadyConfirmed);

        var confirmationResult = await userRepository.ConfirmEmailAsync(user, confirmationCode);
        if (!confirmationResult)
            return Result.Failure(UserErrors.EmailConfirmationFailed);
        
        var isInRole = await userRepository.IsInRoleAsync(user, "Member");
        if (!isInRole)
        {
            var roleAssignmentResult = await userRepository.AddToRoleAsync(user, "Member");

            if (!roleAssignmentResult)
                return Result.Failure(UserErrors.GlobalRoleAssignmentFailed);
        }

        return Result.Success();
    }
}