using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record ClearConversationCommand(Guid ConversationId, string UserId) : IRequest<Result>;