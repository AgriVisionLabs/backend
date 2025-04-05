using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Mapster;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class GetAllFarmsRelatedToUserQueryHandler(IFarmRepository farmRepository,IUserRepository userRepository, IUtilityService utilityService) : IRequestHandler<Queries.GetAllFarmsRelatedToUserQuery, Result<List<FarmResponse>>>
{
    public async Task<Result<List<FarmResponse>>> Handle(Queries.GetAllFarmsRelatedToUserQuery request, CancellationToken cancellationToken)
    {
        var user= await userRepository.FindByIdAsync(request.UserId);
        var userEmail = user!.Email;
        var farms = await farmRepository.GetAllFarmsRelatedToUserAsync(userEmail, cancellationToken);
    
        if (farms.Count == 0)
            return Result.Failure<List<FarmResponse>>(FarmErrors.NoFarmsFound);
    
        var responses = farms.Select(farm => new FarmResponse(utilityService.Encode(farm.Id.ToString()), farm.Name, farm.Area, farm.Location,
                farm.SoilType, farm.CreatedById, farm.FarmMembers.Adapt<List<FarmMembers_Contract>>())).ToList();
        
        return Result.Success(responses);
    }
}