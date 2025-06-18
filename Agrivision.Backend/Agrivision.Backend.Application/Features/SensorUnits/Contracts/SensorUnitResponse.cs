using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.SensorUnits.Contracts;

public record SensorUnitResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string FieldName,
    string Name,
    bool IsOnline,
    DateTime InstallationDate,
    UnitStatus Status,
    DateTime? LastMaintenance,
    DateTime? NextMaintenance,
    string? IpAddress,
    string MacAddress,
    string SerialNumber,
    string FirmWareVersion,
    string AddedById,
    string AddedBy,
    DateTime LastUpdated,
    float Moisture,
    float Temperature,
    float Humidity,
    int BatteryLevel
);