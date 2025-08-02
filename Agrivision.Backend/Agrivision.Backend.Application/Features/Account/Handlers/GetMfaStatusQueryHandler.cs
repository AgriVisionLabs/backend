using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Application.Features.Account.Queries;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Account.Handlers;

public class GetMfaStatusQueryHandler(IUserRepository userRepository) : IRequestHandler<GetMfaStatusQuery, Result<MfaStatusResponse>>
{
    public async Task<Result<MfaStatusResponse>> Handle(GetMfaStatusQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<MfaStatusResponse>(UserErrors.UserNotFound);

        return Result.Success(new MfaStatusResponse(user.TwoFactorEnabled));
    }
}