using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions.Consts;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmUserRoleRepository(CoreDbContext coreDbContext) : IFarmUserRoleRepository
{
    public async Task<IReadOnlyList<FarmUserRole>> AdminGetAllAccessible(string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.Farm)
            .Include(fur => fur.FarmRole)
            .Where(fur => fur.UserId == userId)
            .OrderByDescending(fur => fur.CreatedOn)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FarmUserRole>> GetAllAccessible(string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.Farm)
            .Include(fur => fur.FarmRole)
            .Where(fur => fur.UserId == userId && !fur.IsDeleted && fur.IsActive)
            .OrderByDescending(fur => fur.CreatedOn)
            .ToListAsync(cancellationToken);
    }

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
    
    public async Task<FarmUserRole?> AdminGetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.FarmRole)
            .FirstOrDefaultAsync(fur => fur.FarmId == farmId && fur.UserId == userId, cancellationToken);
    }
    
    public async Task<FarmUserRole?> GetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.FarmUserRoles
            .Include(fur => fur.Farm)
            .Include(fur => fur.FarmRole)
            .FirstOrDefaultAsync(fur => fur.FarmId == farmId && fur.UserId == userId && !fur.IsDeleted && fur.IsActive, cancellationToken);
        
    }

    public async Task AddAsync(FarmUserRole assignment, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmUserRoles.AddAsync(assignment, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task AddRangeAsync(IEnumerable<FarmUserRole> assignments, CancellationToken cancellationToken = default)
    {
        await coreDbContext.FarmUserRoles.AddRangeAsync(assignments, cancellationToken);
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

    public async Task<IReadOnlyList<string>> GetPermissionsForUserInFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default)
    {
        return await (
                from fur in coreDbContext.FarmUserRoles
                join role in coreDbContext.FarmRoles on fur.FarmRoleId equals role.Id
                join claim in coreDbContext.FarmRoleClaims on role.Id equals claim.FarmRoleId
                where fur.UserId == userId
                      && fur.FarmId == farmId
                      && !fur.IsDeleted
                      && claim.ClaimType == CorePermissions.Type
                select claim.ClaimValue
            ).Distinct().ToListAsync(cancellationToken);
    }

    public async Task<bool> AssignUserToRoleAsync(Guid farmId, string userId, string roleName , string createdById, bool isActive = false,
        CancellationToken cancellationToken = default)
    {
        var role = await coreDbContext.FarmRoles
            .FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted, cancellationToken);

        if (role is null)
            return false;

        var assignment = new FarmUserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FarmId = farmId,
            FarmRoleId = role.Id,
            CreatedById = createdById,
            CreatedOn = DateTime.UtcNow // we should remove one of the AssignedAt and CreatedAt but i am too lazy to do it so later k :)
        };

        await coreDbContext.FarmUserRoles.AddAsync(assignment, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}