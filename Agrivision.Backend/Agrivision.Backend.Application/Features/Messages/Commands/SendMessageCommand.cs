using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Messages.Commands;

public record SendMessageCommand(Guid ConversationId, string SenderId, string Content) : IRequest<Result<MessageResponse>>;