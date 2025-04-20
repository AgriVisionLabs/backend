using Agrivision.Backend.Application.Features.FarmRoles.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.FarmRoles.Queries;

public record GetFarmRolesQuery() : IRequest<Result<IReadOnlyList<FarmRoleResponse>>>;