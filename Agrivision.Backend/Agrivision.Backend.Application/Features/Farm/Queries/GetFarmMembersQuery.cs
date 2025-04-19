using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Queries;

public record GetFarmMembersQuery
(
    string RequesterId,
    Guid FarmId
) : IRequest<Result<IReadOnlyList<FarmMemberResponse>>>;