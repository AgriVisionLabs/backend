using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Application.Features.Conversation.Queries;
using Agrivision.Backend.Application.Models;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Handlers;

public class GetConversationByIdQueryHandler(IConversationRepository conversationRepository, IConversationMemberRepository conversationMemberRepository, IUserRepository userRepository) : IRequestHandler<GetConversationByIdQuery, Result<ConversationResponse>>
{
    public async Task<Result<ConversationResponse>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.FindByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result.Failure<ConversationResponse>(ConversationErrors.NotFound);

        var isMember = await conversationMemberRepository.IsMemberAsync(request.RequesterId, request.ConversationId, cancellationToken);
        if (!isMember)
            return Result.Failure<ConversationResponse>(ConversationErrors.AccessDenied);

        var memberEntities = await conversationMemberRepository.GetMembersAsync(request.ConversationId, cancellationToken);
        
        var responses = new List<ReceiverModel>();

        foreach (var member in memberEntities)
        {
            var user = await userRepository.FindByIdAsync(member.UserId);
            if (user is null) continue;

            responses.Add(new ReceiverModel(
                user.Id,
                user.UserName,
                user.Email,
                user.FirstName,
                user.LastName,
                member.IsAdmin
            ));
        }

        var response = new ConversationResponse(
            conversation.Id,
            conversation.Name ?? "",
            conversation.IsGroup,
            conversation.AdminId,
            responses
        );

        return Result.Success(response);
    }
}