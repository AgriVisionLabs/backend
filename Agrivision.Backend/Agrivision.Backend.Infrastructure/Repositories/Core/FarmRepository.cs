using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class FarmRepository(CoreDbContext context, ILogger logger) : IFarmRepository
{
    public async Task<List<Farm>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Farms.ToListAsync(cancellationToken);
    }

    public async Task<List<Farm>> GetAllCreatedByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.Farms
            .Where(farm => farm.CreatedById == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Farm?> GetByIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await context.Farms
            .FirstOrDefaultAsync(farm => farm.Id == farmId, cancellationToken);
    }

    public async Task<Farm?> FindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken = default)
    {
        return await context.Farms
            .FirstOrDefaultAsync(farm => farm.Name == name && farm.CreatedById == userId, cancellationToken);
    }

    public async Task AddAsync(Farm farm, CancellationToken cancellationToken = default)
    {
        try
        {
            await context.Farms.AddAsync(farm, cancellationToken);
            var changes = await context.SaveChangesAsync(cancellationToken);

            if (changes == 0)
            {
                throw new Exception("Failed to save the farm to the database.");
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "An error occurred while adding a farm with Name: {FarmName}, CreatedBy: {CreatedById}", farm.Name, farm.CreatedById);
            throw new ApplicationException("An unexpected error occurred while saving the farm. Please try again later.");
        }
    }
}