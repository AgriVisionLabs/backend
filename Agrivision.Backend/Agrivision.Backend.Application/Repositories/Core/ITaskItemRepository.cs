using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface ITaskItemRepository
{
    // get all by farm Id
    Task<IReadOnlyList<TaskItem>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);

    // get all by field
    Task<IReadOnlyList<TaskItem>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    
    // get all completed tasks by farm id
    Task<IReadOnlyList<TaskItem>> GetAllCompletedByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    
    // get all by field
    Task<IReadOnlyList<TaskItem>> GetAllCompletedByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);
    
    // get due completed tasks by farm id
    Task<IReadOnlyList<TaskItem>> GetAllDueByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default);
    
    // get due by field
    Task<IReadOnlyList<TaskItem>> GetAllDueByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    // find by Id
    Task<TaskItem?> FindByIdAsync(Guid taskItemId, CancellationToken cancellationToken = default);

    // add
    Task AddAsync(TaskItem item, CancellationToken cancellationToken = default);

    // update
    Task UpdateAsync(TaskItem item, CancellationToken cancellationToken = default);

    // remove
    Task RemoveAsync(TaskItem item, CancellationToken cancellationToken = default);

    // exists
    Task<bool> ExistsAsync(Guid taskItemId, CancellationToken cancellationToken = default);
    
    // exists by title and fieldId
    Task<bool> TitleExistsByTitleAndFieldIdAsync(string title, Guid fieldId,
        CancellationToken cancellationToken = default);
}