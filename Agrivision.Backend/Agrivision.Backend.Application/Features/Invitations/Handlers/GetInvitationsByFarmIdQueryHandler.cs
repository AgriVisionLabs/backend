using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Invitations.Contracts;
using Agrivision.Backend.Application.Features.Invitations.Queries;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Invitations.Handlers;

public class GetInvitationsByFarmIdQueryHandler(IFarmRepository farmRepository, IFarmInvitationRepository farmInvitationRepository, IFarmRoleRepository farmRoleRepository, IUserRepository userRepository, IFarmUserRoleRepository farmUserRoleRepository) : IRequestHandler<GetInvitationsByFarmIdQuery, Result<IReadOnlyList<InvitationResponse>>>
{
    public async Task<Result<IReadOnlyList<InvitationResponse>>> Handle(GetInvitationsByFarmIdQuery request, CancellationToken cancellationToken)
    {
        // check if farm exists
        var farm = await farmRepository.FindByIdAsync(request.FarmId, cancellationToken);
        if (farm is null)
            return Result.Failure<IReadOnlyList<InvitationResponse>>(FarmErrors.FarmNotFound);

        // check if user can see invitations (later) (only owner and manager should be able to check them)
        var userRole =
            await farmUserRoleRepository.GetByUserAndFarmAsync(request.FarmId, request.RequesterId, cancellationToken);
        if (userRole is null || (userRole.FarmRole.Name != "Owner" && userRole.FarmRole.Name != "Manager"))
            return Result.Failure<IReadOnlyList<InvitationResponse>>(FarmUserRoleErrors.InsufficientPermissions);
        
        // get invitations
        var invitations = await farmInvitationRepository.GetActiveByFarmIdAsync(farm.Id, cancellationToken);

        // get roles for invitations
        var roleIds = invitations.Select(inv => inv.FarmRoleId).Distinct().ToList();
        var roles = await farmRoleRepository.GetByIdsAsync(roleIds, cancellationToken);
        var roleMap = roles.ToDictionary(r => r.Id, r => r.Name);
        
        // get sender usernames 
        var senderIds = invitations.Select(inv => inv.CreatedById).Distinct().ToList();
        var senders = await userRepository.GetUsersByIdsAsync(senderIds, cancellationToken);
        var senderMap = senders.ToDictionary(u => u.Id, u => u.UserName);
        
        // get receiver
        var receiverEmails = invitations.Select(inv => inv.InvitedEmail).Distinct().ToList();
        var receivers = await userRepository.GetUsersByEmailsAsync(receiverEmails, cancellationToken);
        var receiverMap = receivers.ToDictionary(u => u.Email!, u => u.UserName);

        // project to response
        var response = invitations
            .Select(inv => new InvitationResponse(
                Id: inv.Id,
                FarmId: inv.FarmId,
                SenderId: inv.CreatedById,
                SenderUserName: senderMap.GetValueOrDefault(inv.CreatedById, "Unknown"),
                ReceiverEmail: inv.InvitedEmail,
                ReceiverUserName: receiverMap.GetValueOrDefault(inv.InvitedEmail, "User not registered"),
                ReceiverExists: receiverMap.ContainsKey(inv.InvitedEmail),
                RoleId: inv.FarmRoleId,
                RoleName: roleMap.GetValueOrDefault(inv.FarmRoleId, "Unknown Role"),
                ExpiresAt: inv.ExpiresAt,
                CreatedOn: inv.CreatedOn
            )).ToList();

        return Result.Success<IReadOnlyList<InvitationResponse>>(response);
    }
}