using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class AcceptInvitationCommandHandler(IFarmInvitationRepository farmInvitationRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<AcceptInvitationCommand, Result>
{
    public async Task<Result> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        // get invitation by token 
        var invitation = await farmInvitationRepository.GetByTokenAsync(request.Token, cancellationToken);
        if (invitation is null || invitation.IsAccepted || invitation.ExpiresAt < DateTime.UtcNow || invitation.IsDeleted)
            return Result.Failure(FarmInvitationErrors.InvalidToken);
        
        // get the invited user email
        var invitedUser = await userRepository.FindByEmailAsync(invitation.InvitedEmail);
        if (invitedUser is null)
            return Result.Failure(UserErrors.UserNotFound);
        
        // check if requester is the invited user
        if (request.RequesterId != invitedUser.Id)
            return Result.Failure(FarmInvitationErrors.InvalidToken);
        
        var alreadyAssigned = await farmUserRoleRepository.ExistsAsync(invitation.FarmId, invitedUser.Id, cancellationToken);
        if (alreadyAssigned)
            return Result.Failure(FarmUserRoleErrors.UserAlreadyHasAccess);

        // add the invited user to farm user role with appropriate role
        var assignment = new FarmUserRole
        {
            Id = Guid.NewGuid(),
            FarmId = invitation.FarmId,
            UserId = invitedUser.Id,
            FarmRoleId = invitation.FarmRoleId,
            CreatedById = invitation.CreatedById,
            CreatedOn = DateTime.UtcNow,
        };

        await farmUserRoleRepository.AddAsync(assignment, cancellationToken);
        
        // accept invitation 
        invitation.IsAccepted = true;
        invitation.AcceptedAt = DateTime.UtcNow;
        invitation.UpdatedOn = DateTime.UtcNow;
        invitation.UpdatedById = request.RequesterId;
        
        // update
        await farmInvitationRepository.UpdateAsync(invitation, cancellationToken);

        return Result.Success();
    }
}