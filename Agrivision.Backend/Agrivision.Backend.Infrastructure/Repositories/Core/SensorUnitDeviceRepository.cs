using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class SensorUnitDeviceRepository(CoreDbContext coreDbContext) : ISensorUnitDeviceRepository
{
    public async Task<SensorUnitDevice?> FindByDeviceIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnitDevices
            .FirstOrDefaultAsync(d => d.Id == deviceId && !d.IsDeleted, cancellationToken);
    }

    public async Task<SensorUnitDevice?> FindBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnitDevices
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber && !d.IsDeleted, cancellationToken);
    }

    public async Task UpdateAsync(SensorUnitDevice device, CancellationToken cancellationToken = default)
    {
        coreDbContext.SensorUnitDevices.Update(device);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}