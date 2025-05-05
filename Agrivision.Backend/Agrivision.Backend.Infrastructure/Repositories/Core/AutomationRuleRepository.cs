using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class AutomationRuleRepository(CoreDbContext coreDbContext) : IAutomationRuleRepository
{
    public async Task<IReadOnlyList<AutomationRule>> FindByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.AutomationRules
            .Include(r => r.SensorUnit)
            .Include(r => r.IrrigationUnit)
            .ThenInclude(iu => iu.Field)
            .Where(r => r.SensorUnit.FarmId == farmId && !r.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<AutomationRule?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.AutomationRules
            .Include(r => r.SensorUnit)
            .Include(r => r.IrrigationUnit)
            .ThenInclude(iu => iu.Field)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(AutomationRule rule, CancellationToken cancellationToken = default)
    {
        await coreDbContext.AutomationRules.AddAsync(rule, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AutomationRule rule, CancellationToken cancellationToken = default)
    {
        coreDbContext.AutomationRules.Update(rule);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<AutomationRule?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.AutomationRules
            .Include(r => r.SensorUnit)
            .Include(r => r.IrrigationUnit)
            .ThenInclude(iu => iu.Field)
            .FirstOrDefaultAsync(r => r.Name == name && r.SensorUnit.FarmId == farmId && !r.IsDeleted, cancellationToken);
    }
}