using Agrivision.Backend.Application.Features.Members.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Queries;

public record GetFarmMembersQuery
(
    string RequesterId,
    Guid FarmId
) : IRequest<Result<IReadOnlyList<FarmMemberResponse>>>;