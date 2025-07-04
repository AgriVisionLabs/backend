using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class DiseaseDetectionRepository(CoreDbContext coreDbContext) : IDiseaseDetectionRepository
{
    public async Task<IReadOnlyList<DiseaseDetection>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.DiseaseDetections
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Field)
            .ThenInclude(f => f.Farm)
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Crop)
            .Include(dd => dd.CropDisease)
            .Where(dd => dd.PlantedCrop.Field.FarmId == farmId && !dd.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DiseaseDetection>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.DiseaseDetections
            .Include(dd => dd.CropDisease)
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Field)
            .ThenInclude(f => f.Farm)
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Crop)
            .Where(dd => dd.PlantedCrop.FieldId == fieldId && !dd.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<DiseaseDetection?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.DiseaseDetections
            .Include(dd => dd.CropDisease)
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Field)
            .ThenInclude(f => f.Farm)
            .Include(dd => dd.PlantedCrop)
            .ThenInclude(pc => pc.Crop)
            .FirstOrDefaultAsync(dd => dd.Id == id && !dd.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(DiseaseDetection diseaseDetection, CancellationToken cancellationToken = default)
    {
        await coreDbContext.DiseaseDetections.AddAsync(diseaseDetection, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DiseaseDetection diseaseDetection, CancellationToken cancellationToken = default)
    {
        coreDbContext.DiseaseDetections.Update(diseaseDetection);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}