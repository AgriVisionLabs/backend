using Agrivision.Backend.Application.Features.Invitations.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Invitations.Queries;

public record GetInvitationsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<InvitationResponse>>>;