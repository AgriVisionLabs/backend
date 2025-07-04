using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmRepository(CoreDbContext coreDbContext) : IFarmRepository
{
    // admin method
    public async Task<List<Farm>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms.ToListAsync(cancellationToken);
    }

    // admin method
    public async Task<List<Farm>> AdminGetAllCreatedByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .Where(farm => farm.CreatedById == userId)
            .ToListAsync(cancellationToken);
    }
    
    
    public async Task<List<Farm>> GetAllCreatedByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .Where(farm => farm.CreatedById == userId && !farm.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    // admin method
    public async Task<Farm?> AdminFindByIdAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .FirstOrDefaultAsync(farm => farm.Id == farmId, cancellationToken);
    }

    public async Task<Farm?> FindByIdAsync(Guid farmId, CancellationToken cancellationToken)
    {
        var test = await coreDbContext.Farms
            .FirstOrDefaultAsync(farm => farm.Id == farmId && !farm.IsDeleted, cancellationToken);

        return test;
    }

    public async Task<Farm?> FindByIdWithAllAsync(Guid farmId, CancellationToken cancellationToken)
    {
        var farm = await coreDbContext.Farms
            .Include(f => f.Fields)
                .ThenInclude(field => field.IrrigationUnit)
                .ThenInclude(iu => iu.Device)
            .Include(f => f.Fields)
                .ThenInclude(field => field.IrrigationUnit)
                .ThenInclude(iu => iu.IrrigationEvents)
            .Include(f => f.Fields)
                .ThenInclude(field => field.SensorUnits)
                .ThenInclude(su => su.Device)
            .Include(f => f.Fields)
                .ThenInclude(field => field.TaskItems)
            .Include(f => f.Fields)
                .ThenInclude(field => field.PlantedCrop)
                .ThenInclude(pc => pc.DiseaseDetections)
            .Include(f => f.FarmUserRoles)
            .Include(f => f.FarmInvitations)
            .Include(f => f.AutomationRules)
            .Include(f => f.InventoryItems)
                .ThenInclude(it => it.Transactions)
            .FirstOrDefaultAsync(f => f.Id == farmId && !f.IsDeleted, cancellationToken);

        return farm;
    }

    public async Task<Farm?> AdminFindByIdWithFieldsAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .AsNoTracking()
            .Include(farm => farm.Fields)
            .FirstOrDefaultAsync(farm => farm.Id == farmId, cancellationToken);
    }

    public async Task<Farm?> FindByIdWithFieldsAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .AsNoTracking()
            .Include(farm => farm.Fields)
            .FirstOrDefaultAsync(farm => farm.Id == farmId && !farm.IsDeleted, cancellationToken);
    }
    
    public async Task<Farm?> FindByIdWithFieldsAndRolesAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .AsNoTracking()
            .Include(farm => farm.Fields)
            .Include(farm => farm.FarmUserRoles)
            .FirstOrDefaultAsync(farm => farm.Id == farmId && !farm.IsDeleted, cancellationToken);
    }
    
    // admin method
    public async Task<List<Farm>> AdminFindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .Where(farm => farm.Name == name && farm.CreatedById == userId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Farm?> FindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .FirstOrDefaultAsync(farm => farm.Name == name && farm.CreatedById == userId && !farm.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Farm farm, CancellationToken cancellationToken)
    {
        await coreDbContext.Farms.AddAsync(farm, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Farm farm, CancellationToken cancellationToken)
    {
        coreDbContext.Farms.Update(farm);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Farm farm, CancellationToken cancellationToken)
    {
        coreDbContext.Farms.Remove(farm); 
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await coreDbContext.Farms
            .AnyAsync(f => f.Id == farmId && !f.IsDeleted, cancellationToken);
    }
}