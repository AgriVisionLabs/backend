using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IDiseaseDetectionRepository
{
    // get all by farmId
    Task<IReadOnlyList<DiseaseDetection>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get all by fieldId
    Task<IReadOnlyList<DiseaseDetection>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // get by id
    Task<DiseaseDetection?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // add 
    Task AddAsync(DiseaseDetection diseaseDetection, CancellationToken cancellationToken = default);

    // update
    Task UpdateAsync(DiseaseDetection diseaseDetection, CancellationToken cancellationToken = default);
}