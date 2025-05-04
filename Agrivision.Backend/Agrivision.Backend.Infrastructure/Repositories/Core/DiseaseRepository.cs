

using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class DiseaseRepository(CoreDbContext coreDbContext): IDiseaseRepository
{
    public async Task<Disease?> GetByClassId(int  classId,CancellationToken cancellationToken)
    {
        return await coreDbContext.Diseases.Include(d => d.CropType).SingleOrDefaultAsync(d=>d.ClassIdInModelPredictions==classId,cancellationToken);
    }
}
