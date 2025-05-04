using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ISensorUnitDeviceRepository
{
    Task<SensorUnitDevice?> FindByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default);
    
    // admin get by serial number 
    
    // get by serial number
    Task<SensorUnitDevice?> FindBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);

    // admin get by mac address

    // get by mac address

    // add

    // update
    Task UpdateAsync(SensorUnitDevice device, CancellationToken cancellationToken = default);

    // remove

    // exists
}