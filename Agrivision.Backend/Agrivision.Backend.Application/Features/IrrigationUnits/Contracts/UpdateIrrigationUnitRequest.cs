using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;

public record UpdateIrrigationUnitRequest
(
    string Name,
    IrrigationUnitStatus Status,
    Guid NewFieldId
);