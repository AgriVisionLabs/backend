using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Enums.Core;

using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Utility;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Mapster;
using MediatR;




namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class CreateFarmCommandHandler(IFarmRepository farmRepository
                                     ,IUserRepository userRepository
                                     ,IUtilityService utilityService) : IRequestHandler<CreateFarmCommand, Result<FarmResponse>>
{
    public async Task<Result<FarmResponse>> Handle(CreateFarmCommand request, CancellationToken cancellationToken)
    {
        // check if farm name already used by the user
        var existingFarm =
            await farmRepository.FindByNameAndUserAsync(request.Name, request.CreatedById, cancellationToken);
        if (existingFarm is not null)
            return Result.Failure<FarmResponse>(FarmErrors.DuplicateFarmName);

        // map to farm entity
        var farm = request.Adapt<Domain.Entities.Core.Farm>(); // we do it like that so we can use the generated farm id if we map it while passing it we won't be able to do that 

       
            //to check for dublicate member adding
        var addedMemberEmails = new HashSet<string>();

        var owner = await userRepository.FindByIdAsync(request.CreatedById);

        farm.FarmMembers.Add(new FarmMember { Email = owner!.Email, Role=FarmRoles.Owner });
        addedMemberEmails.Add(owner.Email);
        await userRepository.AddToRoleAsync(owner, FarmRoles.Owner.ToString());


        foreach (var member in request.FarmMembers)
        {
            if (addedMemberEmails.Contains(member.Email))
                continue; // Skip if member already added

            var user = await userRepository.FindByEmailAsync(member.Email);
            if (user is null)
                continue;

            farm.FarmMembers.Add(new FarmMember { Email = user.Email, Role = member.Role });
            addedMemberEmails.Add(user.Email);
            await userRepository.AddToRoleAsync(user, member.Role.ToString());
        }
            // save to the database
            await farmRepository.AddAsync(farm, cancellationToken);
        // convert to response
        return Result.Success(new FarmResponse(utilityService.Encode(farm.Id.ToString()), farm.Name, farm.Area,
            farm.Location, farm.SoilType, farm.CreatedById,farm.FarmMembers.Adapt<List<FarmMembers_Contract>>()));
    }



   

}


