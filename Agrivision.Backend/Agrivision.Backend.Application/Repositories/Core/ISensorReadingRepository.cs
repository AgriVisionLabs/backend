using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ISensorReadingRepository
{
    // get latest by sensor id
    Task<IReadOnlyList<SensorReading>> GetLatestReadingsByUnitIdAsync(Guid unitId, CancellationToken cancellationToken = default);
}