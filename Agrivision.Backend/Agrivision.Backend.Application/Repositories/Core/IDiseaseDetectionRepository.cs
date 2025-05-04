

using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;
public interface IDiseaseDetectionRepository
{
    Task AddRange(List<DiseaseDetection> detections, CancellationToken cancellationToken);
}
