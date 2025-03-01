using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFarmRepository
{
    Task<List<Farm>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<List<Farm>> GetAllByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Farm?> FindByNameAndUserAsync(string name, string userId, CancellationToken cancellationToken = default);
    Task AddAsync(Farm farm, CancellationToken cancellationToken = default);
}