using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Farm.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Services.InvitationTokenGenerator;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Application.Features.Farm.Handlers;

public class InviteMemberCommandHandler(IUserRepository userRepository, IFarmRepository farmRepository, IFarmRoleRepository farmRoleRepository, IFarmInvitationRepository farmInvitationRepository, IInvitationTokenGenerator invitationTokenGenerator, IEmailService emailService, ILogger<InviteMemberCommandHandler> logger) : IRequestHandler<InviteMemberCommand, Result>
{
    public async Task<Result> Handle(InviteMemberCommand request, CancellationToken cancellationToken)
    {
        // check whether email or username
        var isEmail = request.Recipient.Contains('@');
        // verify that the user exists
        var recipient = isEmail
            ? await userRepository.FindByEmailAsync(request.Recipient)
            : await userRepository.FindByUserNameAsync(request.Recipient);
        // if user is null return user not registered error 
        if (recipient is null)
            return Result.Failure(UserErrors.UserNotFound);

        if (recipient.Id == request.SenderId)
            return Result.Failure(FarmInvitationErrors.SelfInvitation);

        // verify the farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure(FarmErrors.FarmNotFound);

        // verify the role exists
        var role = await farmRoleRepository.GetByIdAsync(request.RoleId, cancellationToken);
        if (role is null)
            return Result.Failure(FarmRoleErrors.RoleNotFound);

        // check if invitation already exists
        var existing = await farmInvitationRepository.ExistsAsync(request.FarmId, recipient.Email, cancellationToken);
        if (existing)
            return Result.Failure(FarmInvitationErrors.InvitationAlreadyExists);

        // create the invitation
        var invitation = new FarmInvitation
        {
            Id = Guid.NewGuid(),
            FarmId = farm.Id,
            FarmRoleId = role.Id,
            Token = invitationTokenGenerator.GenerateToken(),
            InvitedEmail = recipient.Email,
            IsAccepted = false,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedOn = DateTime.UtcNow,
            CreatedById = request.SenderId
        };
        
        // save invitation
        await farmInvitationRepository.AddAsync(invitation, cancellationToken);

        // send the email
        await emailService.SendInvitationEmail(farm.Name, request.SenderName, recipient.Email, invitation.Token);
        
        logger.LogInformation("Invitation Token: {token}", invitation.Token);
        
        // return success
        return Result.Success();
    }
}