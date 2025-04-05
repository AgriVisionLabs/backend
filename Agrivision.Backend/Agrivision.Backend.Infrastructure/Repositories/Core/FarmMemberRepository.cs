using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;
namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class FarmMemberRepository(
                                  CoreDbContext context
                                                       ): IFarmMemberRepository
{
    public async Task<FarmMember> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await context.FarmMembers
            .FirstOrDefaultAsync(fm => fm.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException($"FarmMember with ID {id} not found.");
    }
    public async Task<IEnumerable<FarmMember>> GetUser_FarmRoles(string Email)
    {
        return await context.FarmMembers
       .Where(fm => fm.Email == Email)
       .Select(fm => new FarmMember { FarmId=fm.FarmId, Role = fm.Role })
       .ToListAsync();
    }

    public async Task<IEnumerable<FarmMember>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken)
    {
        return await context.FarmMembers
            .Where(fm => fm.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }
    public async Task AddAsync(FarmMember farmMember, CancellationToken cancellationToken)
    {
        if (farmMember == null)
            throw new ArgumentNullException(nameof(farmMember));

        await context.FarmMembers.AddAsync(farmMember, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(FarmMember farmMember, CancellationToken cancellationToken)
    {
        if (farmMember == null)
            throw new ArgumentNullException(nameof(farmMember));

        context.FarmMembers.Remove(farmMember);
        await context.SaveChangesAsync(cancellationToken);
    }
    public async Task<bool> DeleteListByEmails(List<string> emailsList, CancellationToken cancellationToken)
    {
        if (emailsList == null || !emailsList.Any())
        {
            return false;
        }

        var membersToDelete = await context.FarmMembers
            .Where(fm => emailsList.Contains(fm.Email))
            .ToListAsync(cancellationToken);

        if (!membersToDelete.Any())
        {
            return false;
        }

        context.FarmMembers.RemoveRange(membersToDelete);
        var rowsAffected = await context.SaveChangesAsync(cancellationToken);
        return rowsAffected > 0;
    }

    // Other methods...
    
    public async Task<bool> AnyAsync(Func<FarmMember, bool> predicate, CancellationToken cancellationToken)
    {
        return await context.FarmMembers
            .AnyAsync(fm => predicate(fm), cancellationToken);
    }
}


