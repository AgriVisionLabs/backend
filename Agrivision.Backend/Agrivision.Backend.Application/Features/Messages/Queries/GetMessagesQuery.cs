using Agrivision.Backend.Application.Features.Messages.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Messages.Queries;

public record GetMessagesQuery(string RequesterId, Guid ConversationId) : IRequest<Result<IReadOnlyList<MessageResponse>>>;