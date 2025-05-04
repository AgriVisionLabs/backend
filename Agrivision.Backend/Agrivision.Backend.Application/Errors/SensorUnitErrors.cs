using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class SensorUnitErrors
{
    public static readonly Error
        DeviceOffline = new("SensorUnitErrors.DeviceOffline", "The requested device is offline.");

    public static readonly Error FailedToSendCommand = new("SensorUnitErrors.FailedToSendCommand",
        "Failed to send command to the specified irrigation unit.");

    public static readonly Error DeviceUnreachable = new(
        "SensorUnitErrors.DeviceUnreachable",
        "Failed to confirm command execution from device. The device may be offline or unresponsive.");

    public static readonly Error DuplicateNameInFarm = new("SensorUnitErrors.DuplicateNameInFarm",
        "An irrigation unit with the same name already exists in the specified farm.");
    
    public static readonly Error SensorUnitNotFound = new("SensorUnitErrors.SensorUnitNotFound",
        "The specified sensor unit was not found.");
}