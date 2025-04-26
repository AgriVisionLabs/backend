namespace Agrivision.Backend.Application.Features.IrrigationUnits.Commands;

public record ToggleIrrigationUnitCommand
(
    Guid FieldId,
    string RequesterId
);