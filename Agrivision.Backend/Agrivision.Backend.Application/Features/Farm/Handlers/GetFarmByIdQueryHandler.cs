using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetFarmByIdQueryHandler(IFarmUserRoleRepository farmUserRoleRepository, IFarmRepository farmRepository) : IRequestHandler<GetFarmByIdQuery, Result<FarmResponse>>
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

        var response = new FarmResponse(access.Farm.Id, access.Farm.Name, access.Farm.Area, access.Farm.Location,
            access.Farm.SoilType, access.FarmRole.Name, access.Farm.CreatedById, access.FarmRole.Name == "Owner");

        return Result.Success(response);
    }
}