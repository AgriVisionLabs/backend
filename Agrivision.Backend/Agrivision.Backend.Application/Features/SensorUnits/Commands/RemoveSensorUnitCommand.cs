using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Commands;

public record RemoveSensorUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    Guid SensorUnitId,
    string RequesterId
) : IRequest<Result>;
