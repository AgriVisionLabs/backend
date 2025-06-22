using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class IrrigationEventRepository(CoreDbContext coreDbContext) : IIrrigationEventRepository
{
    public async Task<IReadOnlyList<IrrigationEvent>> GetAllByIrrigationUnitIdAsync(Guid irrigationUnitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationEvents
            .Where(ie => ie.IrrigationUnitId == irrigationUnitId && !ie.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IrrigationEvent?> FindLastestByIrrigationUnitIdAsync(Guid irrigationUnitId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.IrrigationEvents
            .Where(ie => ie.IrrigationUnitId == irrigationUnitId && !ie.IsDeleted)
            .OrderByDescending(ie => ie.StartTime)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddAsync(IrrigationEvent irrigationEvent, CancellationToken cancellationToken = default)
    {
        await coreDbContext.IrrigationEvents.AddAsync(irrigationEvent, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(IrrigationEvent irrigationEvent, CancellationToken cancellationToken = default)
    {
        coreDbContext.IrrigationEvents.Update(irrigationEvent);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}