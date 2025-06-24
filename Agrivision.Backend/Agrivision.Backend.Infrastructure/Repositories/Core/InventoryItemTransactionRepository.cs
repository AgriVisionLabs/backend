using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class InventoryItemTransactionRepository(CoreDbContext coreDbContext) : IInventoryItemTransactionRepository
{
    public async Task<IReadOnlyList<InventoryItemTransaction>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItemTransactions
            .Include(transaction => transaction.InventoryItem)
            .Where(transaction => transaction.InventoryItem.FarmId == farmId && !transaction.IsDeleted && transaction.Reason != InventoryTransactionType.ManualAdjustment)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InventoryItemTransaction>> GetAllByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItemTransactions
            .Include(transaction => transaction.InventoryItem)
            .Where(transaction => transaction.InventoryItemId == itemId && !transaction.IsDeleted && transaction.Reason != InventoryTransactionType.ManualAdjustment)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItemTransaction?> GetByIdAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItemTransactions
            .Include(transaction => transaction.InventoryItem)
            .FirstOrDefaultAsync(transaction => transaction.Id == transactionId && !transaction.IsDeleted && transaction.Reason != InventoryTransactionType.ManualAdjustment,
                cancellationToken);
    }

    public async Task AddAsync(InventoryItemTransaction transaction, CancellationToken cancellationToken = default)
    {
        await coreDbContext.InventoryItemTransactions.AddAsync(transaction, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItemTransaction transaction, CancellationToken cancellationToken = default)
    {
        coreDbContext.InventoryItemTransactions.Update(transaction);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}