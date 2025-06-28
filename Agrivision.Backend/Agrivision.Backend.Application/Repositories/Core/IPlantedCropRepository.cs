using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IPlantedCropRepository
{
    // get all by field id
    Task<IReadOnlyList<PlantedCrop>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    
    // get by id
    Task<PlantedCrop?> FindByIdAsync(Guid plantedCropId, CancellationToken cancellationToken = default);

    // add
    Task AddAsync(PlantedCrop plantedCrop, CancellationToken cancellationToken = default);

    // update
    Task UpdateAsync(PlantedCrop plantedCrop, CancellationToken cancellationToken = default);
    
    // find latest by field id 
    Task<PlantedCrop?> FindLatestByFieldId(Guid fieldId, CancellationToken cancellationToken = default);
}