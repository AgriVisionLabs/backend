

using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;
public interface IDiseaseRepository
{
    Task<Disease?> GetByClassId(int classId, CancellationToken cancellationToken);

}
