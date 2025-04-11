using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmRoleClaimRepository
{
    Task<IReadOnlyList<FarmRoleClaim>> AdminGetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmRoleClaim>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<FarmRoleClaim?> AdminGetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FarmRoleClaim?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmRoleClaim>> AdminGetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmRoleClaim>> GetByRoleNameAsync(string roleName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmRoleClaim>> AdminGetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FarmRoleClaim>> GetByRoleIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task AddAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default);
    Task UpdateAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default);
    Task RemoveAsync(FarmRoleClaim farmRoleClaim, CancellationToken cancellationToken = default);
}