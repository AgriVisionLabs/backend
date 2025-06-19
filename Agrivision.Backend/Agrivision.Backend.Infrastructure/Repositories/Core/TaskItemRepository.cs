using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class TaskItemRepository(CoreDbContext coreDbContext) : ITaskItemRepository
{
    public async Task<IReadOnlyList<TaskItem>> GetAllByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.Field.FarmId == farmId && !task.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.FieldId == fieldId && !task.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllCompletedByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.Field.FarmId == farmId && !task.IsDeleted && task.CompletedAt != null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllCompletedByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.FieldId == fieldId && !task.IsDeleted && task.CompletedAt != null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllDueByFarmIdAsync(Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.Field.FarmId == farmId && !task.IsDeleted && task.CompletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllDueByFieldIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.FieldId == fieldId && !task.IsDeleted && task.CompletedAt == null)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<TaskItem>> GetAllByUserIdAndFarmIdAsync(string userId, Guid farmId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .Where(task => task.Field.FarmId == farmId && !task.IsDeleted && (task.AssignedToId == userId || task.ClaimedById == userId || task.ClaimedById == null))
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> FindByIdAsync(Guid taskItemId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .Include(task => task.Field)
            .ThenInclude(field => field.Farm)
            .FirstOrDefaultAsync(task => task.Id == taskItemId && !task.IsDeleted, cancellationToken);
    }

    public async Task AddAsync(TaskItem item, CancellationToken cancellationToken = default)
    {
        await coreDbContext.TaskItems.AddAsync(item, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TaskItem item, CancellationToken cancellationToken = default)
    {
        coreDbContext.TaskItems.Update(item);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveAsync(TaskItem item, CancellationToken cancellationToken = default)
    {
        coreDbContext.TaskItems.Remove(item);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid taskItemId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .AnyAsync(task => task.Id == taskItemId && !task.IsDeleted, cancellationToken);
    }

    public async Task<bool> TitleExistsByTitleAndFieldIdAsync(string title, Guid fieldId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.TaskItems
            .AnyAsync(task =>
                task.Title == title && task.FieldId == fieldId && !task.IsDeleted && task.CompletedAt == null, cancellationToken);
    }
}