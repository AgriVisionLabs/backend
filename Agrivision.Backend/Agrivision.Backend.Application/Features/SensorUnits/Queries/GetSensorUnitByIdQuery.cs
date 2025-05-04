using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Queries;

public record GetSensorUnitByIdQuery
(
    Guid FarmId,
    Guid FieldId,
    Guid SensorUnitId,
    string RequesterId
) : IRequest<Result<SensorUnitResponse>>;