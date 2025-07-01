using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ICropDiseaseRepository
{
    // get all 
    Task<IReadOnlyList<CropDisease>> GetAllAsync(CancellationToken cancellationToken = default);
    
    // get by id 
    Task<CropDisease?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    // get by name 
    Task<CropDisease?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
}