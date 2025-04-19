using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Members.Contracts;
using Agrivision.Backend.Application.Features.Members.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Handlers;

public class GetFarmMembersQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository, IUserRepository userRepository) : IRequestHandler<GetFarmMembersQuery, Result<IReadOnlyList<FarmMemberResponse>>>
{
    public async Task<Result<IReadOnlyList<FarmMemberResponse>>> Handle(GetFarmMembersQuery request, CancellationToken cancellationToken)
    {
        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<FarmMemberResponse>>(FarmErrors.FarmNotFound);
        
        // check if user has access to the farm
        var access = await farmUserRoleRepository.ExistsAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (!access)
            return Result.Failure<IReadOnlyList<FarmMemberResponse>>(FarmErrors.UnauthorizedAction);
        
        // farm user roles
        var farmUserRoles = await farmUserRoleRepository.GetByFarmIdAsync(request.FarmId, cancellationToken);
        
        // get members
        var memberIds = farmUserRoles.Select(fur => fur.UserId).Distinct().ToList();
        var members = await userRepository.GetUsersByIdsAsync(memberIds, cancellationToken);
        var memberMap = members.ToDictionary(m => m.Id); // creates a dictionary from the list of members, where the key is the id and the value is the entire object
        
        // get inviting users
        var senderIds = farmUserRoles.Select(fur => fur.CreatedById).Distinct().ToList();
        var senders = await userRepository.GetUsersByIdsAsync(senderIds, cancellationToken);
        var senderMap = senders.ToDictionary(u => u.Id, u => u.UserName);

        // construct response
        var response = farmUserRoles
            .Where(fur => memberMap.ContainsKey(fur.UserId))
            .Select(fur =>
            {
                var member = memberMap[fur.UserId];
                var invitedByUserName = senderMap.GetValueOrDefault(fur.CreatedById, "Non");

                return new FarmMemberResponse(
                    MemberId: member.Id,
                    FarmId: fur.FarmId,
                    UserName: member.UserName,
                    Email: member.Email!,
                    FirstName: member.FirstName,
                    LastName: member.LastName,
                    RoleId: fur.FarmRoleId,
                    RoleName: fur.FarmRole.Name,
                    JoinedAt: fur.CreatedOn,
                    InvitedByUserName: invitedByUserName,
                    InvitedById: fur.CreatedById,
                    IsOwner: fur.FarmRole.Name == "Owner"
                );
            })
            .ToList();
        
        return Result.Success<IReadOnlyList<FarmMemberResponse>>(response);
    }
}