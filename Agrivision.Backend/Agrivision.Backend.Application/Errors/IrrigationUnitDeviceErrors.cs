using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class IrrigationUnitDeviceErrors
{
    public static readonly Error DeviceNotFound = new("IrrigationDeviceUnit.NotFound", "The requested irrigation unit device was not found.");
    public static readonly Error AlreadyAssigned = new("IrrigationDeviceUnit.AlreadyAssigned", "The device you are trying to assign is already assigned to another field.");
}