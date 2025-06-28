using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class PlantedCropRepository(CoreDbContext coreDbContext) : IPlantedCropRepository
{
    public async Task<IReadOnlyList<PlantedCrop>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.PlantedCrops
            .Include(p => p.Crop)
            .Where(p => p.FieldId == fieldId && !p.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<PlantedCrop?> FindByIdAsync(Guid plantedCropId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.PlantedCrops
            .Include(p => p.Crop)
            .FirstOrDefaultAsync(p => p.Id == plantedCropId && !p.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(PlantedCrop plantedCrop, CancellationToken cancellationToken = default)
    {
        await coreDbContext.PlantedCrops.AddAsync(plantedCrop, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PlantedCrop plantedCrop, CancellationToken cancellationToken = default)
    {
        coreDbContext.PlantedCrops.Update(plantedCrop);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PlantedCrop?> FindLatestByFieldId(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.PlantedCrops
            .Include(p => p.Crop)
            .Where(pc => pc.FieldId == fieldId && !pc.IsDeleted)
            .OrderByDescending(pc => pc.PlantingDate)
            .FirstOrDefaultAsync(cancellationToken);
    }
}