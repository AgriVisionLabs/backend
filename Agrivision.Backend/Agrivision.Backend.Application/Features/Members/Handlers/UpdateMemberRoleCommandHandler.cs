using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Members.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Handlers;

public class UpdateMemberRoleCommandHandler(IFarmUserRoleRepository farmUserRoleRepository, IFarmRoleRepository farmRoleRepository) : IRequestHandler<UpdateMemberRoleCommand, Result>
{
    public async Task<Result> Handle(UpdateMemberRoleCommand request, CancellationToken cancellationToken)
    {
        // check if user is member of farm
        var targetRoleAssignment =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.UserId, cancellationToken);
        if (targetRoleAssignment is null)
            return Result.Failure(FarmUserRoleErrors.UserRoleNotFound);
        
        // check if requester can change and assign the specified role
        var requesterRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if(requesterRole is null || (requesterRole.FarmRole.Name != "Owner" && requesterRole.FarmRole.Name != "Manager"))
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        // managers can't update owners or other managers
        if (requesterRole.FarmRole.Name == "Manager" &&
            (targetRoleAssignment.FarmRole.Name == "Owner" || targetRoleAssignment.FarmRole.Name == "Manager"))
            return Result.Failure(FarmUserRoleErrors.CannotModifyElevatedRoles);
        
        // no self modification
        if (request.RequesterId == request.UserId)
            return Result.Failure(FarmUserRoleErrors.SelfModificationNotAllowed);
        
        // validate new role exists
        var newRole = await farmRoleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (newRole is null)
            return Result.Failure(FarmRoleErrors.RoleNotFound);

        if (newRole.Name == "Owner")
            return Result.Failure(FarmUserRoleErrors.CannotAssignOwnerRole);

        if (newRole.Name == "Manager" && requesterRole.FarmRole.Name != "Owner")
            return Result.Failure(FarmUserRoleErrors.CannotAssignElevatedRoles);
        
        // pass if already in the role
        if (targetRoleAssignment.FarmRoleId == request.RoleId)
            return Result.Success();
        
        // update
        targetRoleAssignment.FarmRoleId = request.RoleId;
        targetRoleAssignment.UpdatedOn = DateTime.UtcNow;
        targetRoleAssignment.UpdatedById = request.RequesterId;
        
        await farmUserRoleRepository.UpdateAsync(targetRoleAssignment, cancellationToken);

        return Result.Success();
    }
}