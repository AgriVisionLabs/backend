using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Members.Contracts;
using Agrivision.Backend.Application.Features.Members.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Handlers;

public class GetFarmMemberRoleQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository) : IRequestHandler<GetFarmMemberQuery, Result<FarmMemberResponse>>
{
    public async Task<Result<FarmMemberResponse>> Handle(GetFarmMemberQuery request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<FarmMemberResponse>(FarmErrors.FarmNotFound);
        
        // check if the user has access to the farm 
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
            return Result.Failure<FarmMemberResponse>(FarmErrors.UnauthorizedAction);
        
        // check if the requested member has access
        var memberRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.MemberId, request.FarmId, cancellationToken);
        if (memberRole is null)
            return Result.Failure<FarmMemberResponse>(FarmUserRoleErrors.UserRoleNotFound);
        
        // get the member
        var member = await userRepository.FindByIdAsync(memberRole.UserId);
        if (member is null)
            return Result.Failure<FarmMemberResponse>(UserErrors.UserNotFound);
        
        // get sender 
        var sender = await userRepository.FindByIdAsync(memberRole.CreatedById);
        if (sender is null)
            return Result.Failure<FarmMemberResponse>(UserErrors.UserNotFound);

        var response = new FarmMemberResponse(member.Id, memberRole.FarmId, member.UserName, member.Email,
            member.FirstName, member.LastName, memberRole.FarmRoleId, memberRole.FarmRole.Name, memberRole.CreatedOn,
            sender.UserName, sender.Id, member.Id == farm.CreatedById);

        return Result.Success(response);
    }
}