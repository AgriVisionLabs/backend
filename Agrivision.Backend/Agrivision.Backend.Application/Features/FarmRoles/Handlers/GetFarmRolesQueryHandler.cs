using Agrivision.Backend.Application.Features.FarmRoles.Contracts;
using Agrivision.Backend.Application.Features.FarmRoles.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.FarmRoles.Handlers;

public class GetFarmRolesQueryHandler(IFarmRoleRepository farmRoleRepository) : IRequestHandler<GetFarmRolesQuery, Result<IReadOnlyList<FarmRoleResponse>>>
{
    public async Task<Result<IReadOnlyList<FarmRoleResponse>>> Handle(GetFarmRolesQuery request, CancellationToken cancellationToken)
    {
        // get the roles
        var roles = await farmRoleRepository.GetAllAsync(cancellationToken);
        if (roles is null)
            throw new Exception("FarmRoles returned null.");
        
        // map to response
        var response = roles.Select(fr =>
            new FarmRoleResponse
            (
                fr.Id,
                fr.Name,
                fr.Description ?? "Non"
            )
        ).ToList();


        return Result.Success<IReadOnlyList<FarmRoleResponse>>(response);
    }
}