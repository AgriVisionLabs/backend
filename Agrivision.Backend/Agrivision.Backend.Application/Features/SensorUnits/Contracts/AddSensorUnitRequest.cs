namespace Agrivision.Backend.Application.Features.SensorUnits.Contracts;

public record AddSensorUnitRequest
(
    string SerialNumber,
    string Name
);