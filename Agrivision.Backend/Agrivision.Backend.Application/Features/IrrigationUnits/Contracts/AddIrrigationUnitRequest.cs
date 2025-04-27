namespace Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;

public record AddIrrigationUnitRequest
(
    string SerialNumber,
    string Name
);