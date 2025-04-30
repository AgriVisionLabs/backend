using Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Queries;

public record GetIrrigationUnitsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<IrrigationUnitResponse>>>;
