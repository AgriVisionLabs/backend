using Agrivision.Backend.Application.Features.FarmRoles.Contracts;
using Agrivision.Backend.Application.Features.FarmRoles.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.FarmRoles.Handlers;

public class GetFarmRolesPermissionsQueryHandler(IFarmRoleClaimRepository farmRoleClaimRepository) : IRequestHandler<GetFarmRolesPermissionsQuery, Result<IReadOnlyList<FarmRolePermissionsResponse>>>
{
    public async Task<Result<IReadOnlyList<FarmRolePermissionsResponse>>> Handle(GetFarmRolesPermissionsQuery request, CancellationToken cancellationToken)
    {
        // get permissions
        var permissions = await farmRoleClaimRepository.GetAllAsync(cancellationToken);
        
        // map to response
        var response = permissions
            .GroupBy(p => p.FarmRole.Name)
            .Select(group => new FarmRolePermissionsResponse(
                RoleName: group.Key,
                Permissions: group.Select(p => p.ClaimValue).ToList()
            ))
            .ToList();

        return Result.Success<IReadOnlyList<FarmRolePermissionsResponse>>(response);
    }
}