using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class InventoryItemRepository(CoreDbContext coreDbContext) : IInventoryItemRepository
{
    public async Task<IReadOnlyList<InventoryItem>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItems
            .Where(item => item.FarmId == farmId && !item.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<InventoryItem?> GetByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItems
            .FirstOrDefaultAsync(item => item.Id == itemId && !item.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        await coreDbContext.InventoryItems.AddAsync(item, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(InventoryItem item, CancellationToken cancellationToken = default)
    {
        coreDbContext.InventoryItems.Update(item);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByFarmIdAndItemName(Guid farmId, string itemName, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.InventoryItems
            .AnyAsync(item => item.FarmId == farmId && item.Name == itemName && !item.IsDeleted, cancellationToken);
    }
}