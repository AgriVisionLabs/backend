using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record ToggleAdminStatusCommand
(    
    string RequesterId,
    Guid ConversationId,
    string TargetUserId
) : IRequest<Result<ToggleAdminStatusResponse>>;