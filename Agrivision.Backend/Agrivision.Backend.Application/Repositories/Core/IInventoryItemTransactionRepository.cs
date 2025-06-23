using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IInventoryItemTransactionRepository
{
    // get all by farm id
    Task<IReadOnlyList<InventoryItemTransaction>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get all by item id
    Task<IReadOnlyList<InventoryItemTransaction>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default);

    // get by id
    Task<InventoryItemTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default);

    // add 
    Task AddAsync(InventoryItemTransaction transaction, CancellationToken cancellationToken = default);

    // update
    Task UpdateAsync(InventoryItemTransaction transaction, CancellationToken cancellationToken = default);
}