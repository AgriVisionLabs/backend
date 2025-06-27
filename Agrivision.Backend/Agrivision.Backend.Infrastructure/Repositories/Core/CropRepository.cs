using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class CropRepository(CoreDbContext coreDbContext) : ICropRepository
{
    public async Task<IReadOnlyList<Crop>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Crops
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.CropType)
            .ToListAsync(cancellationToken);
    }

    public async Task<Crop?> FindByIdAsync(Guid cropId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Crops
            .FirstOrDefaultAsync(c => c.Id == cropId && !c.IsDeleted, cancellationToken);
    }

    public async Task<Crop?> FindByCropTypeAsync(CropType cropType, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.Crops
            .FirstOrDefaultAsync(c => c.CropType == cropType, cancellationToken);
    }
}