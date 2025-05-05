
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class CropRepository(CoreDbContext coreDbContext):ICropRepository
{
    public async Task<CropType?> GetByNameAsync(CropTypes name,CancellationToken cancellationToken=default)
    {
    
        return await coreDbContext.CropTypes
            .FirstOrDefaultAsync(c=>c.Name==name, cancellationToken);
    
    }
    public async Task<CropType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {

        return await coreDbContext.CropTypes
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    }
}
