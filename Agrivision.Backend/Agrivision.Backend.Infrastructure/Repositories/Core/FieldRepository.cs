using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FieldRepository(CoreDbContext coreDbContext) : IFieldRepository
{
    public async Task<List<Field>> AdminGetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .Where(field => field.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<List<Field>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .Where(field => field.FarmId == farmId && !field.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<Field?> AdminFindByIdWithFarmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .Include(field => field.Farm)
            .FirstOrDefaultAsync(field => field.Id == id, cancellationToken);
    }

    public async Task<Field?> FindByIdWithFarmAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .Include(field => field.Farm)
            .FirstOrDefaultAsync(field => field.Id == id && !field.IsDeleted, cancellationToken);
    }
    
    public async Task<Field?> AdminFindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .FirstOrDefaultAsync(field => field.Id == id, cancellationToken);
    }

    public async Task<Field?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .FirstOrDefaultAsync(field => field.Id == id && !field.IsDeleted, cancellationToken);
    }

    public async Task<List<Field>> AdminFindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .AsNoTracking()
            .Where(field => field.Name == name && field.FarmId == farmId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Field?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Fields
            .AsNoTracking()
            .FirstOrDefaultAsync(field => field.Name == name && field.FarmId == farmId && !field.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(Field field, CancellationToken cancellationToken = default)
    {
        await coreDbContext.Fields.AddAsync(field, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Field field, CancellationToken cancellationToken = default)
    {
        coreDbContext.Fields.Update(field);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(Field field, CancellationToken cancellationToken = default)
    {
        coreDbContext.Fields.Remove(field);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}