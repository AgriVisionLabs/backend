using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class ReadNotificationRepository(CoreDbContext coreDbContext) : IReadNotificationRepository
{
    public async Task AddAsync(ReadNotification readNotification, CancellationToken cancellationToken = default)
    {
        await coreDbContext.ReadNotifications.AddAsync(readNotification, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReadNotification>> GetAllReadNotificationsAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.ReadNotifications
            .Where(rn => rn.NotificationId == notificationId && rn.UserId == userId)
            .OrderByDescending(rn => rn.ReadAt)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<HashSet<Guid>> GetReadNotificationIdsByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.ReadNotifications
            .Where(rn => rn.UserId == userId)
            .Select(rn => rn.NotificationId)
            .ToHashSetAsync(cancellationToken);
    }

    public async Task<ReadNotification?> GetReadNotificationByIdAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.ReadNotifications
            .FirstOrDefaultAsync(rn => rn.NotificationId == notificationId && rn.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsNotificationReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.ReadNotifications.AnyAsync(
            rn => rn.NotificationId == notificationId && rn.UserId == userId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ReadNotification> readNotifications, CancellationToken cancellationToken = default)
    {
        await coreDbContext.ReadNotifications.AddRangeAsync(readNotifications, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}