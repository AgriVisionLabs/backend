using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ICropRepository
{
    // get all 
    Task<IReadOnlyList<Crop>> GetAllAsync(CancellationToken cancellationToken = default);

    // get by id
    Task<Crop?> FindByIdAsync(Guid cropId, CancellationToken cancellationToken = default);

    // get by enum value
    Task<Crop?> FindByCropTypeAsync(CropType cropType, CancellationToken cancellationToken = default);
}