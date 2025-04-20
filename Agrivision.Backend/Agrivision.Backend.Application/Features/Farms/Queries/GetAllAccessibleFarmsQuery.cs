using Agrivision.Backend.Application.Features.Farms.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farms.Queries;

public record GetAllAccessibleFarmsQuery
(
    string RequesterId
) : IRequest<Result<IReadOnlyList<FarmResponse>>>;