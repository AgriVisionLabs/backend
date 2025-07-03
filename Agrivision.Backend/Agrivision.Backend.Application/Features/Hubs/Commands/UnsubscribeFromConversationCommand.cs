using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Hubs.Commands;

public record UnsubscribeFromConversationCommand(string ConnectionId, Guid ConversationId) : IRequest<Result>;