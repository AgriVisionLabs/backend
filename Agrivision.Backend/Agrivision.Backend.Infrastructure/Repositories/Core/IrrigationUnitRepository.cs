using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class IrrigationUnitRepository(CoreDbContext coreDbContext) : IIrrigationUnitRepository
{
    public async Task<IReadOnlyList<IrrigationUnit>> AdminGetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IrrigationUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Where(u => !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IrrigationUnit?> AdminFindByIdAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);
    }

    public async Task<IrrigationUnit?> FindByIdAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.Id == unitId && !u.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<IrrigationUnit>> AdminFindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Where(u => u.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<IrrigationUnit>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Where(u => u.FarmId == farmId && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IrrigationUnit?> AdminFindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.FieldId == fieldId, cancellationToken);
    }

    public async Task<IrrigationUnit?> FindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.FieldId == fieldId && !u.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(IrrigationUnit unit, CancellationToken cancellationToken = default)
    {
        await coreDbContext.IrrigationUnits.AddAsync(unit, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IrrigationUnit unit, CancellationToken cancellationToken = default)
    {
        coreDbContext.IrrigationUnits.Update(unit);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(IrrigationUnit unit, CancellationToken cancellationToken = default)
    {
        coreDbContext.Remove(unit);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return coreDbContext.IrrigationUnits
            .AnyAsync(u => u.Id == unitId, cancellationToken);
    }

    public async Task<IrrigationUnit?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits
            .FirstOrDefaultAsync(u => u.Name == name && u.FarmId == farmId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits.AnyAsync(u => u.Name == name && u.FarmId == farmId && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationUnits.AnyAsync(u => u.FieldId == fieldId && !u.IsDeleted, cancellationToken);
    }
}