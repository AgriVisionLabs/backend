using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Invitations.Commands;

public record AcceptInvitationCommand
(
    string RequesterId,
    string Token
) : IRequest<Result>;