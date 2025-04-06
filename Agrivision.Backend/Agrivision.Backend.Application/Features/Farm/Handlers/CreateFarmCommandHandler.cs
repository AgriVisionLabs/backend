using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class CreateFarmCommandHandler(IFarmRepository farmRepository) : IRequestHandler<CreateFarmCommand, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(CreateFarmCommand request, CancellationToken cancellationToken)
    {
        // check if farm name already used by the user
        var existingFarm =
            await farmRepository.FindByNameAndUserAsync(request.Name, request.CreatedById, cancellationToken);
        if (existingFarm is not null)
            return Result.Failure<FarmResponse>(FarmErrors.DuplicateFarmName);

        // map to farm entity
        var farm = new Domain.Entities.Core.Farm
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Area = request.Area,
            Location = request.Location,
            SoilType = request.SoilType,
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.CreatedById,
            IsDeleted = false
        };
        
        // save to the database
        await farmRepository.AddAsync(farm, cancellationToken);
        // convert to response
        return Result.Success(new FarmResponse(farm.Id, farm.Name, farm.Area,
            farm.Location, farm.SoilType, farm.CreatedById));
    }
}