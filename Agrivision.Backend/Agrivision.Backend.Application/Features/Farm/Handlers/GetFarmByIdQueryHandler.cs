using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetFarmByIdQueryHandler(IFarmRepository farmRepository) : IRequestHandler<GetFarmByIdQuery, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(GetFarmByIdQuery request, CancellationToken cancellationToken)
    {
        var farm = await farmRepository.FindByIdAsync(request.Id, cancellationToken);
        
        if (farm is null)
            return Result.Failure<FarmResponse>(FarmErrors.FarmNotFound);

        var response = new FarmResponse(farm.Id, farm.Name, farm.Area, farm.Location,
            farm.SoilType, farm.CreatedById);

        return Result.Success(response);
    }
}