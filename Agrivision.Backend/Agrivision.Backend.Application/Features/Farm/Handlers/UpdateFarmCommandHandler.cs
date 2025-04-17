using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class UpdateFarmCommandHandler(IFarmRepository farmRepository) : IRequestHandler<UpdateFarmCommand, Result>
{
    public async Task<Result> Handle(UpdateFarmCommand request, CancellationToken cancellationToken)
    {
        // check if the farm exists
        var farm = await farmRepository.FindByIdAsync(request.Id, cancellationToken);
        if (farm == null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // only owner can update farm profile
        if (request.UpdatedById != farm.CreatedById)
            return Result.Failure(FarmErrors.UnauthorizedAction);

        // check if farm name is used by this user
        var existingFarm = await farmRepository.FindByNameAndUserAsync(request.Name, request.UpdatedById, cancellationToken);
        if (existingFarm is not null && existingFarm.Id != farm.Id && !existingFarm.IsDeleted)
            return Result.Failure(FarmErrors.DuplicateFarmName);
        
        // update farm 
        farm.Name = request.Name;
        farm.Area = request.Area;
        farm.Location = request.Location;
        farm.SoilType = request.SoilType;
        farm.UpdatedOn = DateTime.UtcNow;
        farm.UpdatedById = request.UpdatedById;
        
        
        // update farm in the database
        await farmRepository.UpdateAsync(farm, cancellationToken);
        
        // create a response 
        var response = new FarmResponse(farm.Id, farm.Name, farm.Area, farm.Location, farm.SoilType, "Owner", farm.CreatedById, true);

        return Result.Success();
    }
}