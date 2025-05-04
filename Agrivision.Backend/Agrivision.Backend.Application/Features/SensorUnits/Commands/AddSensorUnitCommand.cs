using Agrivision.Backend.Application.Features.SensorUnits.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.SensorUnits.Commands;

public record AddSensorUnitCommand
(
    Guid FarmId,
    Guid FieldId,
    string RequesterId,
    string RequesterName,
    string SerialNumber,
    string Name
) : IRequest<Result<SensorUnitResponse>>;