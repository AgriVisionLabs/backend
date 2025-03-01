using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetAllFarmsQueryHandler(IFarmRepository farmRepository) : IRequestHandler<GetAllFarmsQuery, Result<List<FarmResponse>>>
{
    public async Task<Result<List<FarmResponse>>> Handle(GetAllFarmsQuery request, CancellationToken cancellationToken)
    {
        var farms = await farmRepository.GetAllByUserIdAsync(request.UserId, cancellationToken);

        return farms.Count == 0 ? Result.Failure<List<FarmResponse>>(FarmErrors.NoFarmsFound) : Result.Success(farms.Adapt<List<FarmResponse>>());
    }
}