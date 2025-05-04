using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class SensorReadingRepository(CoreDbContext coreDbContext) : ISensorReadingRepository
{
    public async Task<IReadOnlyList<SensorReading>> GetLatestReadingsByUnitIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        // get all active sensor configs for the unit
        var configIds = await coreDbContext.SensorConfigurations
            .Where(sc => sc.DeviceId == deviceId && sc.IsActive && !sc.IsDeleted)
            .Select(sc => sc.Id)
            .ToListAsync(cancellationToken);

        // for each config get the latest reading
        var latestReadings = await coreDbContext.SensorReadings
            .Where(r => configIds.Contains(r.SensorConfigurationId))
            .GroupBy(r => r.SensorConfigurationId)
            .Select(g => g.OrderByDescending(r => r.TimeStamp).First())
            .ToListAsync(cancellationToken);
        
        return latestReadings;
    }
}