using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Repositories.Core;
public interface ICropRepository
{
    Task<CropType?> GetByNameAsync(CropTypes name, CancellationToken cancellationToken = default);
    Task<CropType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
