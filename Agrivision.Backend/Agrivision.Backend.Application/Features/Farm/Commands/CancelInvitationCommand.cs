using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Commands;

public record CancelInvitationCommand
(
    Guid InvitationId,
    string RequesterId
) : IRequest<Result>;