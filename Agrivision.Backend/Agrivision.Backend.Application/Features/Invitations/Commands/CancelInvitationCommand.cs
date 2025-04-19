using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Invitations.Commands;

public record CancelInvitationCommand
(
    Guid FarmId,
    Guid InvitationId,
    string RequesterId
) : IRequest<Result>;