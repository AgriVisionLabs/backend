using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IIrrigationEventRepository
{
    // get all by irrigation unit id
    Task<IReadOnlyList<IrrigationEvent>> GetAllByIrrigationUnitIdAsync(Guid irrigationUnitId, CancellationToken cancellationToken = default);
    
    // get latest by irrigation unit id
    Task<IrrigationEvent?> FindLastestByIrrigationUnitIdAsync(Guid irrigationUnitId, CancellationToken cancellationToken = default);
    
    // add
    Task AddAsync(IrrigationEvent irrigationEvent, CancellationToken cancellationToken = default);
}