using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmUserRoleRepository(CoreDbContext coreDbContext) : IFarmUserRoleRepository
{
    public async Task<List<FarmUserRole>> AdminGetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .Where(fur => fur.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<FarmUserRole>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .Where(fur => fur.FarmId == farmId && !fur.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<FarmUserRole>> GetActiveByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .Where(fur => fur.FarmId == farmId && !fur.IsDeleted && fur.IsActive)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<FarmUserRole?> AdminGetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .FirstOrDefaultAsync(fur => fur.FarmId == farmId && fur.UserId == userId, cancellationToken);
    }
    
    public async Task<FarmUserRole?> GetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .FirstOrDefaultAsync(fur => fur.FarmId == farmId && fur.UserId == userId && !fur.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(FarmUserRole assignment, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmUserRoles.AddAsync(assignment, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> AdminExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .AnyAsync(fur => fur.FarmId == farmId && fur.UserId == userId, cancellationToken);
    }
    
    public async Task<bool> ExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .AnyAsync(fur => fur.FarmId == farmId && fur.UserId == userId && !fur.IsDeleted, cancellationToken);
    }

    public async Task RemoveAsync(FarmUserRole assignment, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmUserRoles.Remove(assignment);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}