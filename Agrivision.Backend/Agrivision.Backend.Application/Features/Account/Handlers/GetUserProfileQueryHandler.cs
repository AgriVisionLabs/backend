using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Application.Features.Account.Queries;
using MediatR;
using Agrivision.Backend.Application.Repositories.Identity;
using Mapster;

namespace Agrivision.Backend.Application.Features.Account.Handlers;
public class GetUserProfileQueryHandler(IUserRepository userRepository): IRequestHandler<GetUserProfileQuery, Result<UserProfileResponse>>
{
    public async Task<Result<UserProfileResponse>> Handle(GetUserProfileQuery request,CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.userId);
       
        return Result.Success(user.Adapt<UserProfileResponse>());
    }
}
