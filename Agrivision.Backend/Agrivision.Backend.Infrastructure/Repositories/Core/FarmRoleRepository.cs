using System.Security.Cryptography;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmRoleRepository(CoreDbContext coreDbContext) : IFarmRoleRepository
{
    public async Task<List<FarmRole>> AdminGetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles.ToListAsync(cancellationToken);
    }

    public async Task<List<FarmRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<FarmRole?> AdminGetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<FarmRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);
    }

    public async Task<List<FarmRole>> AdminGetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .Where(r => r.Name == name)
            .ToListAsync(cancellationToken);
    }

    public async Task<FarmRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(FarmRole farmRole, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmRoles.AddAsync(farmRole, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmRoles
            .AnyAsync(r => r.Name == name && !r.IsDeleted, cancellationToken);
    }

    public async Task RemoveAsync(FarmRole farmRole, CancellationToken cancellationToken = default)
    {
        coreDbContext.FarmRoles.Remove(farmRole);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}