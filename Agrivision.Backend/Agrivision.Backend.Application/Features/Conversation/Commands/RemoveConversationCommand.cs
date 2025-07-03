using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Commands;

public record RemoveConversationCommand(string RequesterId, Guid ConversationId) : IRequest<Result>;