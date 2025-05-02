using Agrivision.Backend.Application.Features.Members.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Members.Queries;

public record GetFarmMemberQuery
(
    string RequesterId,
    Guid FarmId,
    string MemberId
) : IRequest<Result<FarmMemberResponse>>;