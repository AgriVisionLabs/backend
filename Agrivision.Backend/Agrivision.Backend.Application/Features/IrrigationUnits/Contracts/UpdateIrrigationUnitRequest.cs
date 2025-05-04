using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;

public record UpdateIrrigationUnitRequest
(
    string Name,
    UnitStatus Status,
    Guid NewFieldId
);