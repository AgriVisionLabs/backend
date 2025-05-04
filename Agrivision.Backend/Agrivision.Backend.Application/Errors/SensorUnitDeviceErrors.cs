using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class SensorUnitDeviceErrors
{
    public static readonly Error DeviceNotFound = new("SensorUnitDeviceErrors.NotFound", "The requested irrigation unit device was not found.");
    public static readonly Error AlreadyAssigned = new("SensorUnitDeviceErrors.AlreadyAssigned", "The device you are trying to assign is already assigned to another field.");
}