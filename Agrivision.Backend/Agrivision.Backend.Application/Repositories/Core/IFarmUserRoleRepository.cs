using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmUserRoleRepository
{
    Task<List<FarmUserRole>> AdminGetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<List<FarmUserRole>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<List<FarmUserRole>> GetActiveByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<FarmUserRole?> AdminGetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task<FarmUserRole?> GetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task AddAsync(FarmUserRole assignment, CancellationToken cancellationToken = default);
    Task<bool> AdminExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task RemoveAsync(FarmUserRole assignment, CancellationToken cancellationToken = default);
}