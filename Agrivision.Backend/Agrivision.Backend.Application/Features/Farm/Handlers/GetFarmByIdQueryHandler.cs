using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetFarmByIdQueryHandler(IFarmRepository farmRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetFarmByIdQuery, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(GetFarmByIdQuery request, CancellationToken cancellationToken)
    {
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        
        if (farm is null)
            return Result.Failure<FarmResponse>(FarmErrors.FarmNotFound);
        
        // check if user has access to the farm
        var access =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (access is null)
            return Result.Failure<FarmResponse>(FarmErrors.UnauthorizedAction);

        var response = new FarmResponse(farm.Id, farm.Name, farm.Area, farm.Location,
            farm.SoilType, access.FarmRole.Name, farm.CreatedById);

        return Result.Success(response);
    }
}