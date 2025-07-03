using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Application.Features.Conversation.Queries;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Interfaces.Identity;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Handlers;

public class GetAccessibleConversationsQueryHandler(IConversationRepository conversationRepository, IConversationMemberRepository conversationMemberRepository, IUserRepository userRepository) : IRequestHandler<GetAccessibleConversationsQuery, Result<IReadOnlyList<ConversationResponse>>>
{
    public async Task<Result<IReadOnlyList<ConversationResponse>>> Handle(GetAccessibleConversationsQuery request, CancellationToken cancellationToken)
    {
        var conversationIds = await conversationMemberRepository
            .GetConversationIdsForUserAsync(request.RequesterId, cancellationToken);

        var conversations = new List<ConversationResponse>();

        foreach (var conversationId in conversationIds)
        {
            var conversation = await conversationRepository.FindByIdAsync(conversationId, cancellationToken);
            if (conversation == null)
                continue;

            var members = await conversationMemberRepository.GetMembersAsync(conversationId, cancellationToken);
            var userModels = new List<IApplicationUser>();

            foreach (var member in members)
            {
                var user = await userRepository.FindByIdAsync(member.UserId);
                if (user != null)
                    userModels.Add(user);
            }
            
            var adminMap = members.ToDictionary(m => m.UserId, m => m.IsAdmin);

            var convoResponse = new ConversationResponse(
                conversation.Id,
                conversation.Name ?? "",
                conversation.IsGroup,
                conversation.AdminId,
                userModels.Select(u => new ReceiverModel(
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    adminMap.TryGetValue(u.Id, out var isAdmin) && isAdmin
                )).ToList()
            );

            conversations.Add(convoResponse);
        }

        return Result.Success<IReadOnlyList<ConversationResponse>>(conversations);
    }
}