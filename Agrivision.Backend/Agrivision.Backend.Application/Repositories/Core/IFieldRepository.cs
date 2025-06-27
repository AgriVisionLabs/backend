using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IFieldRepository
{
    Task<List<Field>> AdminGetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<List<Field>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    Task<Field?> AdminFindByIdWithFarmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Field?> FindByIdWithFarmAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Field?> FindByIdWithAllAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Field?> AdminFindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Field?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Field>> AdminFindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default);
    Task<Field?> FindByNameAndFarmIdAsync(string name, Guid farmId, CancellationToken cancellationToken = default);
    Task AddAsync(Field field, CancellationToken cancellationToken = default);
    Task UpdateAsync(Field field, CancellationToken cancellationToken = default);
    Task RemoveAsync(Field field, CancellationToken cancellationToken = default);
}