using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Members.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Members.Handlers;

public class RevokeAccessCommandHandler(IFarmUserRoleRepository farmUserRoleRepository, ILogger<RevokeAccessCommandHandler> logger) : IRequestHandler<RevokeAccessCommand, Result>
{
    public async Task<Result> Handle(RevokeAccessCommand request, CancellationToken cancellationToken)
    {
        // check if the user has access to the farm
        var farmUserRole =
            await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.UserId, request.FarmId, cancellationToken);
        if (farmUserRole is null)
        {
            logger.LogWarning("User {RequesterId} tried to revoke access for user {UserId} on farm {FarmId}", request.RequesterId, request.UserId, request.FarmId);
            return Result.Success();
        }
        
        if (request.RequesterId == request.UserId)
            return Result.Failure(FarmUserRoleErrors.SelfRevokeNotAllowed);
        
        var requesterRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.RequesterId, request.FarmId, cancellationToken);
        if (requesterRole is null || (requesterRole.FarmRole.Name != "Owner" && requesterRole.FarmRole.Name != "Manager"))
            return Result.Failure(FarmErrors.UnauthorizedAction);
        
        farmUserRole.IsDeleted = true;
        farmUserRole.DeletedOn = DateTime.UtcNow;
        farmUserRole.DeletedById = request.RequesterId;
        farmUserRole.IsActive = false;
        farmUserRole.UpdatedOn = DateTime.UtcNow;
        farmUserRole.UpdatedById = request.RequesterId;

        await farmUserRoleRepository.UpdateAsync(farmUserRole, cancellationToken);
        
        return Result.Success();
    }
}