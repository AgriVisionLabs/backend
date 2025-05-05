

using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class DiseaseDetectionRepository(CoreDbContext coreDbContext): IDiseaseDetectionRepository
{
    public async Task AddRange(List<DiseaseDetection> detections,CancellationToken cancellationToken)
    {
        await coreDbContext.AddRangeAsync(detections, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}
