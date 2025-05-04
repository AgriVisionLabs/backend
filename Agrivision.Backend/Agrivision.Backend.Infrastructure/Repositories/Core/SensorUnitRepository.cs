using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class SensorUnitRepository(CoreDbContext coreDbContext) : ISensorUnitRepository
{
        public async Task<IReadOnlyList<SensorUnit>> AdminGetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SensorUnit>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Where(u => !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<SensorUnit?> AdminFindByIdAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.Id == unitId, cancellationToken);
    }

    public async Task<SensorUnit?> FindByIdAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Include(u => u.Device)
            .FirstOrDefaultAsync(u => u.Id == unitId && !u.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<SensorUnit>> AdminFindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Where(u => u.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SensorUnit>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Include(u => u.Device)
            .Where(u => u.FarmId == farmId && !u.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<SensorUnit?> AdminFindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .FirstOrDefaultAsync(u => u.FieldId == fieldId, cancellationToken);
    }

    public async Task<SensorUnit?> FindByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .Include(u => u.Farm)
            .Include(u => u.Field)
            .Include(u => u.Device)
            .FirstOrDefaultAsync(u => u.FieldId == fieldId && !u.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(SensorUnit unit, CancellationToken cancellationToken = default)
    {
        await coreDbContext.SensorUnits.AddAsync(unit, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SensorUnit unit, CancellationToken cancellationToken = default)
    {
        coreDbContext.SensorUnits.Update(unit);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(SensorUnit unit, CancellationToken cancellationToken = default)
    {
        coreDbContext.Remove(unit);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(Guid unitId, CancellationToken cancellationToken = default)
    {
        return coreDbContext.SensorUnits
            .AnyAsync(u => u.Id == unitId && !u.IsDeleted, cancellationToken);
    }

    public async Task<SensorUnit?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits
            .FirstOrDefaultAsync(u => u.Name == name && u.FarmId == farmId && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits.AnyAsync(u => u.Name == name && u.FarmId == farmId && !u.IsDeleted, cancellationToken);
    }

    public async Task<bool> ExistsByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.SensorUnits.AnyAsync(u => u.FieldId == fieldId && !u.IsDeleted, cancellationToken);
    }
}