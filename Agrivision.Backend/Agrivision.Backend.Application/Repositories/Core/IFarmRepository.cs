using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmRepository
{
    Task<List<Farm>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Farm>> AdminGetAllCreatedByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<List<Farm>> GetAllCreatedByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Farm?> AdminFindByIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<Farm?> FindByIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<Farm?> AdminFindByIdWithFieldsAsync(Guid farmId, CancellationToken cancellationToken);
    Task<Farm?> FindByIdWithFieldsAsync(Guid farmId, CancellationToken cancellationToken);
    Task<List<Farm>> AdminFindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken = default);
    Task<Farm?> FindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Farm farm, CancellationToken cancellationToken = default);
    Task UpdateAsync(Farm farm, CancellationToken cancellationToken = default);
    Task RemoveAsync(Farm farm, CancellationToken cancellationToken = default);
}