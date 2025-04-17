using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;


namespace Agrivision.Backend.Application.Features.Account.Handlers;
public class ChangePasswordCommandHandler(IUserRepository userRepository) : IRequestHandler<ChangePasswordCommand, Result>
{
    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user =await userRepository.FindByIdAsync(request.UserId);

        var success = await userRepository.ChangePasswordAsync(user!, request.CurrentPassword, request.NewPassword);

        return Result.Success();

    }

}
