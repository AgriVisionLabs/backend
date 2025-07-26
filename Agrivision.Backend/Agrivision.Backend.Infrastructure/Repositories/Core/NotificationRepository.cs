using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class NotificationRepository(CoreDbContext coreDbContext) : INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> GetAllNotificationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cleared = await coreDbContext.ClearedNotifications
            .OrderByDescending(cn => cn.ClearedAt)
            .FirstOrDefaultAsync(cn => cn.UserId == userId, cancellationToken);

        var clearedAt = cleared?.ClearedAt ?? DateTime.MinValue;

        return await coreDbContext.Notifications
            .Include(n => n.ReadNotifications)
            .Where(n => n.UserIds.Contains(userId)
                        && !n.IsDeleted
                        && n.CreatedOn > clearedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        await coreDbContext.Notifications.AddAsync(notification, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}