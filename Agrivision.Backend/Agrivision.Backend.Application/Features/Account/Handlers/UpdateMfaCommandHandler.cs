using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Handlers;

public class UpdateMfaCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateMfaCommand, Result>
{
    public async Task<Result> Handle(UpdateMfaCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure(UserErrors.UserNotFound);
        
        user.TwoFactorEnabled = request.IsEnabled;
        
        await userRepository.UpdateAsync(user);
        
        return Result.Success();
    }
}