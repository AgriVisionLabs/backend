using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class IrrigationUnitDeviceRepository(CoreDbContext coreDbContext) : IIrrigationUnitDeviceRepository
{
    public async Task<IrrigationUnitDevice?> FindBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnitDevices
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber && !d.IsDeleted, cancellationToken);
    }

    public async Task UpdateAsync(IrrigationUnitDevice device, CancellationToken cancellationToken = default)
    {
        coreDbContext.IrrigationUnitDevices.Update(device);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}