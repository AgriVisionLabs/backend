using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Commands;

public record SubscribeToConversationCommand
(
    Guid ConversationId,
    string ConnectionId,
    string UserId
) : IRequest<Result>;