using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class CropDiseaseRepository(CoreDbContext coreDbContext) : ICropDiseaseRepository
{
    public async Task<IReadOnlyList<CropDisease>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await coreDbContext.CropDiseases
            .Where(cd => !cd.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<CropDisease?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.CropDiseases
            .FirstOrDefaultAsync(cd => cd.Id == id, cancellationToken);
    }

    public async Task<CropDisease?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.CropDiseases
            .FirstOrDefaultAsync(cd => cd.Name == name, cancellationToken);
    }
}