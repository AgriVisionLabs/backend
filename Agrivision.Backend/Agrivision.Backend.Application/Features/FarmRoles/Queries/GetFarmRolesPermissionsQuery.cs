using Agrivision.Backend.Application.Features.FarmRoles.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.FarmRoles.Queries;

public record GetFarmRolesPermissionsQuery() : IRequest<Result<IReadOnlyList<FarmRolePermissionsResponse>>>;