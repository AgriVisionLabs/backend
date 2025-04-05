
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Mapster;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;
public class UpdateFarmCommandHandler(
    IFarmRepository farmRepository,
    IFarmMemberRepository farmMemberRepository,
    IUserRepository userRepository,
    IUtilityService utilityService) : IRequestHandler<UpdateFarmCommand, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(UpdateFarmCommand request, CancellationToken cancellationToken)
    {
        // Decode and validate farm ID
        if (!utilityService.TryDecode(request.EncodedFarmId, out var decodedId) ||
            !Guid.TryParse(decodedId, out var farmId))
            return Result.Failure<FarmResponse>(FarmErrors.InvalidFarmId);


        // Get existing farm
        var farm = await farmRepository.GetByIdAsync(farmId, cancellationToken);
        if (farm is null)
            return Result.Failure<FarmResponse>(FarmErrors.FarmNotFound);



        // Check if new name is already used by another farm for this user
        var existingFarm = await farmRepository.FindByNameAndUserAsync(
            request.Name,
            farm.CreatedById,
            cancellationToken);

        if (existingFarm is not null && existingFarm.Id != farmId)
            return Result.Failure<FarmResponse>(FarmErrors.DuplicateFarmName);

        // Update basic farm properties
        farm.Name = request.Name;
        farm.Area = request.Area;
        farm.Location = request.Location;
        farm.SoilType = request.SoilType;


    
        var currentMembers = farm.FarmMembers.ToDictionary(m => m.Email);
        var requestedMember = request.FarmMembers.ToDictionary(m => m.Email, m => m.Role);


        var membersToRemove = currentMembers
            .Where(cm => cm.Value.Role != FarmRoles.Owner && !requestedMember.ContainsKey(cm.Key))
            .Select(cm => cm.Key)
            .ToList();

        await farmMemberRepository.DeleteListByEmails(membersToRemove, cancellationToken);
      
       

        // Process additions
        foreach (var member in requestedMember)
        {

            if (!currentMembers.TryGetValue(member.Key, out var existingMember))
            {
                var user = await userRepository.FindByEmailAsync(member.Key);
                if (user is null)
                    continue;

                await farmMemberRepository.AddAsync(
                    new FarmMember { FarmId=farmId,
                                     Role = member.Value,
                                     Email = member.Key
                                                         }, cancellationToken);
                continue;
            }
            if(existingMember.Role != member.Value)
                existingMember.Role = member.Value;
        }

        // Update farm in database
         await farmRepository.UpdateAsync(farm, cancellationToken);

        // Refresh farm members for the response (since we modified them separately)
        var farmMembers = await farmMemberRepository.GetByFarmIdAsync(farmId, cancellationToken);
        farm.FarmMembers = farmMembers?.ToList() ?? new List<FarmMember>();

        // Convert to response
        return Result.Success(new FarmResponse(
            utilityService.Encode(farm.Id.ToString()),
            farm.Name,
            farm.Area,
            farm.Location,
            farm.SoilType,
            farm.CreatedById,
            farm.FarmMembers.Adapt<List<FarmMembers_Contract>>()
        ));

    }
}
