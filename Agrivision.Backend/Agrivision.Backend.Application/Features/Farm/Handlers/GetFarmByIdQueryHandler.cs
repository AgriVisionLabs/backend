using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Features.Farm.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetFarmByIdQueryHandler(IFarmRepository farmRepository, IUtilityService utilityService) : IRequestHandler<GetFarmByIdQuery, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(GetFarmByIdQuery request, CancellationToken cancellationToken)
    {
        if (!utilityService.TryDecode(request.EncodedFarmId, out var decodedId) ||
            !Guid.TryParse(decodedId, out var farmId))
            return Result.Failure<FarmResponse>(FarmErrors.InvalidFarmId);

        var farm = await farmRepository.GetByIdAsync(farmId, cancellationToken);

        if (farm is null)
            return Result.Failure<FarmResponse>(FarmErrors.FarmNotFound);

        var response = new FarmResponse(utilityService.Encode(farm.Id.ToString()), farm.Name, farm.Area, farm.Location,
            farm.SoilType, farm.CreatedById);

        return Result.Success(response);
    }
}