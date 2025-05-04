using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.SensorUnits.Contracts;

public record UpdateSensorUnitRequest
(
    string Name,
    UnitStatus Status,
    Guid NewFieldId
);