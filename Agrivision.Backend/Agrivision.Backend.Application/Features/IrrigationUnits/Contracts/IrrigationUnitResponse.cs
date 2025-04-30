using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Features.IrrigationUnits.Contracts;

public record IrrigationUnitResponse
(
    Guid Id,
    Guid FarmId,
    Guid FieldId,
    string FieldName,
    string Name,
    DateTime InstallationDate,
    IrrigationUnitStatus Status,
    DateTime? LastMaintenance,
    DateTime? NextMaintenance,
    string? IpAddress,
    string MacAddress,
    string FirmWareVersion,
    string AddedById,
    string AddedBy,
    TimeSpan LastOperationDuration,
    DateTime LastUpdated
);