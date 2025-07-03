using Agrivision.Backend.Application.Features.Conversation.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Conversation.Queries;

public record GetAccessibleConversationsQuery(string RequesterId) : IRequest<Result<IReadOnlyList<ConversationResponse>>>;