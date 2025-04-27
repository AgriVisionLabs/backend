using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IIrrigationUnitDeviceRepository
{
    // Admin get all
    
    // get all
    
    // admin get by id
    
    // get by id 
    
    // admin get by serial number 
    
    // get by serial number
    Task<IrrigationUnitDevice?> FindBySerialNumberAsync(string serialNumber,
        CancellationToken cancellationToken = default);

    // admin get by mac address

    // get by mac address

    // add

    // update
    Task UpdateAsync(IrrigationUnitDevice device, CancellationToken cancellationToken = default);

    // remove

    // exists


}