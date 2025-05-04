using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Commands;

public record UpdateSensorUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    Guid SensorUnitId,
    string RequesterId,
    string Name,
    UnitStatus Status,
    Guid NewFieldId
) : IRequest<Result>;