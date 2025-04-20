using Agrivision.Backend.Application.Features.Farms.Contracts;
using Agrivision.Backend.Application.Features.Farms.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Handlers;

public class GetAllAccessibleFarmsQueryHandler(IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<GetAllAccessibleFarmsQuery, Result<IReadOnlyList<FarmResponse>>>
{
    public async Task<Result<IReadOnlyList<FarmResponse>>> Handle(GetAllAccessibleFarmsQuery request, CancellationToken cancellationToken)
    {
        // get all farms id user have access to
        var accessible = await farmUserRoleRepository.GetAllAccessible(request.RequesterId, cancellationToken);
        
        // map it to response
        var response = accessible.Select(fur => new FarmResponse(
                fur.Farm.Id,
                fur.Farm.Name,
                fur.Farm.Area,
                fur.Farm.Location,
                fur.Farm.SoilType,
                fur.FarmRole.Name,
                fur.Farm.CreatedById,
                fur.FarmRole.Name == "Owner"))
            .DistinctBy(f => f.FarmId)
            .ToList();

        return Result.Success<IReadOnlyList<FarmResponse>>(response);
    }
}