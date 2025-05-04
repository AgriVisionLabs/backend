using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ISensorUnitRepository
{
    // admin get all 
    Task<IReadOnlyList<SensorUnit>> AdminGetAllAsync(CancellationToken cancellationToken = default);

    // get all 
    Task<IReadOnlyList<SensorUnit>> GetAllAsync(CancellationToken cancellationToken = default);

    // admin get by id
    Task<SensorUnit?> AdminFindByIdAsync(Guid unitId, CancellationToken cancellationToken = default);

    // get by id 
    Task<SensorUnit?> FindByIdAsync(Guid unitId, CancellationToken cancellationToken = default);

    // admin get by farm id
    Task<IReadOnlyList<SensorUnit>> AdminFindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get by farm id 
    Task<IReadOnlyList<SensorUnit>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // admin get by field id 
    Task<SensorUnit?> AdminFindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // get by field id 
    Task<SensorUnit?> FindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // add 
    Task AddAsync(SensorUnit unit, CancellationToken cancellationToken = default);
    
    // update
    Task UpdateAsync(SensorUnit unit, CancellationToken cancellationToken = default);

    // remove
    Task RemoveAsync(SensorUnit unit, CancellationToken cancellationToken = default);

    // exits
    Task<bool> ExistsAsync(Guid unitId, CancellationToken cancellationToken = default);
    
    // find by name and farm (since name is unique per farm)
    Task<SensorUnit?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default);
    
    // exists by name and farm
    Task<bool> ExistsByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default);
    
    // exits by fieldId
    Task<bool> ExistsByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
}