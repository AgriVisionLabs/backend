using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetAllFarmsCreatedByUserIdQueryHandler(IFarmRepository farmRepository, IUtilityService utilityService) : IRequestHandler<Queries.GetAllFarmsCreatedByUserIdQuery, Result<List<FarmResponse>>>
{
    public async Task<Result<List<FarmResponse>>> Handle(Queries.GetAllFarmsCreatedByUserIdQuery request, CancellationToken cancellationToken)
    {
        var farms = await farmRepository.GetAllCreatedByUserIdAsync(request.UserId, cancellationToken);
    
        if (farms.Count == 0)
            return Result.Failure<List<FarmResponse>>(FarmErrors.NoFarmsFound);
    
        var responses = farms.Select(farm => new FarmResponse(utilityService.Encode(farm.Id.ToString()), farm.Name, farm.Area, farm.Location,
                farm.SoilType, farm.CreatedById, farm.FarmMembers.Adapt<IEnumerable<CreateFarm_FarmMembers>>().ToList())).ToList();
        
        return Result.Success(responses);
    }
}