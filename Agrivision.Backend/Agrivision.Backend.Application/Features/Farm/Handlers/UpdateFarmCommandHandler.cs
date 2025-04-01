
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


        // Convert to comparable collections
        var currentMembers = farm.FarmMembers
            .Select(m => new { m.Email, m.Role })
            .ToList();
        var requestedMembers = request.FarmMembers
            .Select(m => new { m.Email, m.Role })
            .ToList();

        // Using Except operator to find members to remove (excluding owner)
        var membersToRemove = currentMembers
            .Where(cm => cm.Role != FarmRoles.Owner)
            .Except(requestedMembers)
            .Select(m => farm.FarmMembers.First(fm => fm.Email == m.Email && fm.Role == m.Role))
            .ToList();

        // Using Except operator to find members to add
        var membersToAdd = requestedMembers
            .Except(currentMembers)
            .Select(m => new FarmMember { FarmId = farmId, Email = m.Email, Role = m.Role })
            .ToList();


        // Process removals
        foreach (var member in membersToRemove)
        {
            await farmMemberRepository.DeleteAsync(member, cancellationToken);
            var user = await userRepository.FindByEmailAsync(member.Email);
            if (user != null)
            {
                // Optional: Check if the role exists in other farms before removing
                var hasRoleInOtherFarms = await farmMemberRepository.AnyAsync(
                    fm => fm.Email == member.Email &&
                          fm.Role == member.Role &&
                          fm.FarmId != farmId,
                    cancellationToken);

                if (!hasRoleInOtherFarms)
                {
                  //  await userRepository.RemoveFromRoleAsync(user, member.Role.ToString());
                }
            }
        }

        // Process additions
        foreach (var member in membersToAdd)
        {
            var user = await userRepository.FindByEmailAsync(member.Email);
            if (user is null)
                continue;

            member.Email = user.Email; // Ensure consistency with actual user email
            await farmMemberRepository.AddAsync(member, cancellationToken);
            await userRepository.AddToRoleAsync(user, member.Role.ToString());
        }

        // Update farm in database
      //  await farmRepository.UpdateAsync(farm, cancellationToken);

        // Refresh farm members for the response (since we modified them separately)
     //   farm.FarmMembers = await farmMemberRepository.GetByFarmIdAsync(farmId, cancellationToken);
        
        
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
