using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.FarmRoles.Contracts;
using Agrivision.Backend.Application.Features.FarmRoles.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.FarmRoles.Handlers;

public class GetFarmRolePermissionsQueryHandler(IFarmRoleClaimRepository farmRoleClaimRepository, IFarmRoleRepository farmRoleRepository) : IRequestHandler<GetFarmRolePermissionsQuery, Result<FarmRolePermissionsResponse>>
{
    public async Task<Result<FarmRolePermissionsResponse>> Handle(GetFarmRolePermissionsQuery request, CancellationToken cancellationToken)
    {
        // verify the role exists
        var role = await farmRoleRepository.GetByNameAsync(request.RoleName, cancellationToken);
        if (role is null)
            return Result.Failure<FarmRolePermissionsResponse>(FarmRoleErrors.RoleNotFound);
        
        // get the permissions
        var permissions = await farmRoleClaimRepository.GetByRoleIdAsync(role.Id, cancellationToken);
        
        // map to response
        var response = new FarmRolePermissionsResponse(role.Name, permissions.Select(frc => frc.ClaimValue).ToList());

        return Result.Success(response);
    }
}