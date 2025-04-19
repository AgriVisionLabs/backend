

using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Agrivision.Backend.Application.Repositories.Identity;

namespace Agrivision.Backend.Application.Features.Account.Handlers;
public class UpdateUserProfileCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserProfileCommand, Result>
{
    public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);

        user!.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.UserName = request.UserName;
        user.PhoneNumber= request.PhoneNumber;
        user.UpdatedAt= DateTime.UtcNow;

        await userRepository.UpdateAsync(user);

        return Result.Success();


    }
}
