using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class CancelInvitationCommandHandler(IFarmInvitationRepository farmInvitationRepository) : IRequestHandler<CancelInvitationCommand, Result>
{
    public async Task<Result> Handle(CancelInvitationCommand request, CancellationToken cancellationToken)
    {
        // get invitation
        var invitation = await farmInvitationRepository.GetByIdAsync(request.InvitationId, cancellationToken);
        if (invitation is null)
            return Result.Failure(FarmInvitationErrors.InvitationNotFound);
        
        // verify user is author (aka can delete)
        if (invitation.CreatedById != request.RequesterId)
            return Result.Failure(FarmInvitationErrors.UnauthorizedAction);
        
        // check if accepted
        if (invitation.IsAccepted)
            return Result.Failure(FarmInvitationErrors.AlreadyAccepted);
        
        // check if already cancelled
        if (invitation.IsDeleted)
            return Result.Success();
        
        // delete invitation
        invitation.IsDeleted = true;
        invitation.DeletedById = request.RequesterId;
        invitation.DeletedOn = DateTime.UtcNow;
        invitation.UpdatedById = request.RequesterId;
        invitation.UpdatedOn = DateTime.UtcNow;
        
        // save to database
        await farmInvitationRepository.UpdateAsync(invitation, cancellationToken);
        
        return Result.Success();
    }
}