using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class IrrigationUnitErrors
{
    public static readonly Error UnitNotAssigned = new("IrrigationUnit.NotAssigned", "The requested irrigation unit is not assigned to a field yet.");

    public static readonly Error
        DeviceOffline = new("IrrigationUnit.DeviceOffline", "The requested device is offline.");

    public static readonly Error FailedToSendCommand = new("IrrigationUnit.FailedToSendCommand",
        "Failed to send command to the specified irrigation unit.");

    public static readonly Error DeviceUnreachable = new(
        "IrrigationUnit.DeviceUnreachable",
        "Failed to confirm command execution from device. The device may be offline or unresponsive.");

    public static readonly Error DuplicateNameInFarm = new("IrrigationUnit.DuplicateNameInFarm",
        "An irrigation unit with the same name already exists in the specified farm.");
}