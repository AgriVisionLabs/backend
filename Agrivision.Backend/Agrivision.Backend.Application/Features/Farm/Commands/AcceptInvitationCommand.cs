using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Commands;

public record AcceptInvitationCommand
(
    string RequesterId,
    string Token
) : IRequest<Result>;