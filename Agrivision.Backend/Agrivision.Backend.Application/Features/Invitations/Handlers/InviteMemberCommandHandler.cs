using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Invitations.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Services.InvitationTokenGenerator;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Invitations.Handlers;

public class InviteMemberCommandHandler(IUserRepository userRepository, IFarmRepository farmRepository, IFarmRoleRepository farmRoleRepository, IFarmInvitationRepository farmInvitationRepository, IInvitationTokenGenerator invitationTokenGenerator, IEmailService emailService, ILogger<InviteMemberCommandHandler> logger, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<InviteMemberCommand, Result>
{
    public async Task<Result> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        // clean up expired invites
        await farmInvitationRepository.CleanupExpiredInvitationsAsync(cancellationToken);
        
        // check if user can invite
        var userRole = await farmUserRoleRepository.FindByUserIdAndFarmIdAsync(request.SenderId, request.FarmId, cancellationToken);
        if (userRole is null || (userRole.FarmRole.Name != "Owner" && userRole.FarmRole.Name != "Manager"))
            return Result.Failure(FarmUserRoleErrors.InsufficientPermissions);
        
        // check whether email or username
        var isEmail = request.Recipient.Contains('@');
        
        // verify that user isn't inviting himself
        if (isEmail && request.Recipient == request.SenderEmail) 
            return Result.Failure(FarmInvitationErrors.SelfInvitation);
        
        // check if the user exists
        var recipient = isEmail
            ? await userRepository.FindByEmailAsync(request.Recipient)
            : await userRepository.FindByUserNameAsync(request.Recipient);
        
        // if with username and null then return error
        if (!isEmail && recipient is null)
            return Result.Failure(UserErrors.UserNotFound);
        
        var invitedEmail = recipient is null && isEmail ? request.Recipient : recipient.Email;
        
        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);
        
        // check if user already have access to the farm 
        if (recipient is not null)
        {
            if (await farmUserRoleRepository.ExistsAsync(farm.Id, recipient.Id, cancellationToken))
                return Result.Failure(FarmUserRoleErrors.UserAlreadyHasAccess);
        }

        // verify the role exists
        var role = await farmRoleRepository.GetByNameAsync(request.RoleName, cancellationToken);
        if (role is null)
            return Result.Failure(FarmRoleErrors.RoleNotFound);
        if (role.Name == "Owner")
            return Result.Failure(FarmInvitationErrors.CannotInviteAsOwner);

        // check if invitation already exists
        var existing = await farmInvitationRepository.ExistsAsync(request.FarmId, invitedEmail, cancellationToken);
        if (existing)
            return Result.Failure(FarmInvitationErrors.InvitationAlreadyExists);

        // create the invitation
        var invitation = new FarmInvitation
        {
            Id = Guid.NewGuid(),
            FarmId = farm.Id,
            FarmRoleId = role.Id,
            Token = invitationTokenGenerator.GenerateToken(),
            InvitedEmail = invitedEmail,
            IsAccepted = false,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.SenderId
        };
        
        // save invitation
        await farmInvitationRepository.AddAsync(invitation, cancellationToken);

        // send the email
        await emailService.SendInvitationEmail(farm.Name, request.SenderName, invitedEmail, invitation.Token);
        
        logger.LogInformation("Invitation Token: {token}", invitation.Token);
        
        // return success
        return Result.Success();
    }
}