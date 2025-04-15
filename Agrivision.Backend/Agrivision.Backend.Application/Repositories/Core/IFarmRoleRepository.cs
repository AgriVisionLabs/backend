using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmRoleRepository
{
    Task<List<FarmRole>> AdminGetAllAsync(CancellationToken cancellationToken = default);
    Task<List<FarmRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FarmRole?> AdminGetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FarmRole?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<List<FarmRole>> AdminGetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<FarmRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(FarmRole farmRole, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task RemoveAsync(FarmRole farmRole, CancellationToken cancellationToken = default);
}