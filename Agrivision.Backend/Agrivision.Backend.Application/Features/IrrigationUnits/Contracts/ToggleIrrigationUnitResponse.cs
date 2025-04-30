namespace Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;

public record ToggleIrrigationUnitResponse
(
    bool IsOn,
    string ToggledById,
    string ToggledBy
);