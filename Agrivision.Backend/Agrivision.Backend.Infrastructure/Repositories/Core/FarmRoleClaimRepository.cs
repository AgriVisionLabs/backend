using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmRoleClaimRepository(CoreDbContext coreDbContext) : IFarmRoleClaimRepository
{
    public async Task<IReadOnlyList<FarmRoleClaim>> AdminGetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmRoleClaim>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .Where(rc => !rc.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<FarmRoleClaim?> AdminGetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .FirstOrDefaultAsync(rc => rc.Id == id, cancellationToken);
    }

    public async Task<FarmRoleClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .FirstOrDefaultAsync(rc => rc.Id == id && !rc.IsDeleted, cancellationToken);
    }

    public async Task<IReadOnlyList<FarmRoleClaim>> AdminGetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .Where(rc => rc.FarmRole.Name == roleName)
            .ToListAsync(cancellationToken);
    }
    

    public async Task<IReadOnlyList<FarmRoleClaim>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .Where(rc => rc.FarmRole.Name == roleName && !rc.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmRoleClaim>> AdminGetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .Where(rc => rc.FarmRoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmRoleClaim>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoleClaims
            .Include(rc => rc.FarmRole)
            .Where(rc => rc.FarmRoleId == roleId && !rc.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmRoleClaims.AddAsync(farmRoleClaim, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmRoleClaims.Update(farmRoleClaim);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmRoleClaims.Remove(farmRoleClaim);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}