using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmUserRoleRepository
{
    Task<IReadOnlyList<FarmUserRole>> AdminGetAllAccessible(string userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmUserRole>> GetAllAccessible(string userId, CancellationToken cancellationToken = default);
    Task<List<FarmUserRole>> AdminGetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<List<FarmUserRole>> GetByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<FarmUserRole?> AdminGetByUserAndFarmAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task<FarmUserRole?> FindByUserIdAndFarmIdAsync(string userId, Guid farmId, CancellationToken cancellationToken = default);
    Task AddAsync(FarmUserRole assignment, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<FarmUserRole> assignments, CancellationToken cancellationToken = default);
    Task<bool> AdminExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid farmId, string userId, CancellationToken cancellationToken = default);
    Task UpdateAsync(FarmUserRole assignment, CancellationToken cancellationToken = default);
    Task RemoveAsync(FarmUserRole assignment, CancellationToken cancellationToken = default);
    Task<bool> AssignUserToRoleAsync(Guid farmId, string userId, string roleName , string createdById, bool isActive = false, CancellationToken cancellationToken = default);
}