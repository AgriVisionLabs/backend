using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class IrrigationUnitDeviceErrors
{
    public static readonly Error DeviceNotFound = new("IrrigationDeviceUnit.NotFound", "The requested irrigation unit device was not found.");
}