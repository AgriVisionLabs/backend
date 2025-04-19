using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Commands;

public record RevokeAccessCommand
(
    string RequesterId,
    Guid FarmId,
    string UserId
) : IRequest<Result>;