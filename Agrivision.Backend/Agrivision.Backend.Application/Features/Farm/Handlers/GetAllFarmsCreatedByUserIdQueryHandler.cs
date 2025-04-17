using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetAllFarmsCreatedByUserIdQueryHandler(IFarmRepository farmRepository) : IRequestHandler<Queries.GetAllFarmsCreatedByUserIdQuery, Result<List<FarmResponse>>>
{
    public async Task<Result<List<FarmResponse>>> Handle(Queries.GetAllFarmsCreatedByUserIdQuery request, CancellationToken cancellationToken)
    {
        var farms = await farmRepository.GetAllCreatedByUserIdAsync(request.UserId, cancellationToken);
    
        var responses = farms
            .Select(farm => new FarmResponse(farm.Id, farm.Name, farm.Area, farm.Location,
                farm.SoilType, "Owner", farm.CreatedById)).ToList();
        
        return Result.Success(responses);
    }
}