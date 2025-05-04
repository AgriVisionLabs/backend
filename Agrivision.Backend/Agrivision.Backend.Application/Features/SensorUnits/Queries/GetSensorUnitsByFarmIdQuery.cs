using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Queries;

public record GetSensorUnitsByFarmIdQuery
(
    Guid FarmId,
    string RequesterId
) : IRequest<Result<IReadOnlyList<SensorUnitResponse>>>;