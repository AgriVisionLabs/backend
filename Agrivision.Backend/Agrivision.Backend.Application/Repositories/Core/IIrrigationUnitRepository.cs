using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IIrrigationUnitRepository
{
    // admin get all 
    Task<IReadOnlyList<IrrigationUnit>> AdminGetAllAsync(CancellationToken cancellationToken = default);

    // get all 
    Task<IReadOnlyList<IrrigationUnit>> GetAllAsync(CancellationToken cancellationToken = default);

    // admin get by id
    Task<IrrigationUnit?> AdminFindByIdAsync(Guid unitId, CancellationToken cancellationToken = default);

    // get by id 
    Task<IrrigationUnit?> FindByIdAsync(Guid unitId, CancellationToken cancellationToken = default);

    // admin get by farm id
    Task<IReadOnlyList<IrrigationUnit>> AdminFindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get by farm id 
    Task<IReadOnlyList<IrrigationUnit>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // admin get by field id 
    Task<IrrigationUnit?> AdminFindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // get by field id 
    Task<IrrigationUnit?> FindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // add 
    Task AddAsync(IrrigationUnit unit, CancellationToken cancellationToken = default);
    
    // update
    Task UpdateAsync(IrrigationUnit unit, CancellationToken cancellationToken = default);

    // remove
    Task RemoveAsync(IrrigationUnit unit, CancellationToken cancellationToken = default);

    // exits
    Task<bool> ExistsAsync(Guid unitId, CancellationToken cancellationToken = default);
}