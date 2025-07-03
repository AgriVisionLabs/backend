using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Conversation.Commands;
using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Interfaces.Identity;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Handlers;

public class AddConversationMemberCommandHandler(IConversationRepository conversationRepository, IConversationMemberRepository conversationMemberRepository, IUserRepository userRepository, IConversationInviteLogRepository inviteLogRepository, IConversationBroadcaster broadcaster) : IRequestHandler<AddConversationMembersCommand, Result<ConversationResponse>>
{
    public async Task<Result<ConversationResponse>> Handle(AddConversationMembersCommand request, CancellationToken cancellationToken)
    {
        var convo = await conversationRepository.FindByIdAsync(request.ConversationId, cancellationToken);
        if (convo is null || !convo.IsGroup)
            return Result.Failure<ConversationResponse>(ConversationErrors.InvalidGroup);

        var isMember = await conversationMemberRepository.IsMemberAsync(request.RequesterId, convo.Id, cancellationToken);
        if (!isMember)
            return Result.Failure<ConversationResponse>(ConversationErrors.AccessDenied);

        var existingMembers = await conversationMemberRepository.GetMembersAsync(convo.Id, cancellationToken);
        var existingUserIds = existingMembers.Select(m => m.UserId).ToHashSet();

        var newMembers = new List<ConversationMember>();
        var newInvites = new List<ConversationInviteLog>();
        var allUsers = new List<IApplicationUser>();
        
        var alreadyAddedUserIds = new HashSet<string>(existingUserIds);

        foreach (var identifier in request.MembersList.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var user = identifier.Contains('@')
                ? await userRepository.FindByEmailAsync(identifier)
                : await userRepository.FindByUserNameAsync(identifier);

            if (user is null || user.Id == request.RequesterId || 
                existingUserIds.Contains(user.Id) || 
                newMembers.Any(m => m.UserId == user.Id))
                continue;

            newMembers.Add(new ConversationMember
            {
                ConversationId = convo.Id,
                UserId = user.Id,
                JoinedAt = DateTime.UtcNow,
                InvitedById = request.RequesterId
            });

            newInvites.Add(new ConversationInviteLog
            {
                Id = Guid.NewGuid(),
                ConversationId = convo.Id,
                InviteeId = user.Id,
                InviterId = request.RequesterId,
                CreatedOn = DateTime.UtcNow,
                CreatedById = request.RequesterId
            });

            allUsers.Add(user);
        }

        if (!newMembers.Any())
            return Result.Failure<ConversationResponse>(ConversationErrors.EmptyParticipantList); // could be a more specific error

        await conversationMemberRepository.AddRangeAsync(newMembers, cancellationToken);
        await inviteLogRepository.AddRangeAsync(newInvites, cancellationToken);

        var existingUsers = await Task.WhenAll(existingUserIds.Select(id => userRepository.FindByIdAsync(id)));
        var fullUsers = allUsers.Concat(existingUsers.Where(u => u != null)!);

        var memberMap = (await conversationMemberRepository.GetMembersAsync(convo.Id, cancellationToken))
            .ToDictionary(m => m.UserId, m => m.IsAdmin);

        var response = new ConversationResponse(
            convo.Id,
            convo.Name ?? "",
            convo.IsGroup,
            convo.AdminId,
            fullUsers.Select(u => new ReceiverModel(
                u!.Id,
                u.UserName,
                u.Email,
                u.FirstName,
                u.LastName,
                memberMap.TryGetValue(u.Id, out var isAdmin) && isAdmin
            )).ToList()
        );

        await Task.WhenAll(fullUsers.Select(u =>
            broadcaster.BroadcastConversationUpdatedAsync(u!.Id, response)));

        return Result.Success(response);
    }
}