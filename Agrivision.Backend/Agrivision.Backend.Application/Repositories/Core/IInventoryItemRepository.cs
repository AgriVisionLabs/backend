using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IInventoryItemRepository
{
    // get all by farm id
    Task<IReadOnlyList<InventoryItem>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get by id
    Task<InventoryItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    // add
    Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default);

    // update
    Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default);

}