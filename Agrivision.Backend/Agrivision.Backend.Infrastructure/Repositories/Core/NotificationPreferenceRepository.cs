using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class NotificationPreferenceRepository(CoreDbContext coreDbContext) : INotificationPreferenceRepository
{
    public async Task AddAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default)
    {
        await coreDbContext.NotificationPreferences.AddAsync(notificationPreference, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.NotificationPreferences
            .Where(np => np.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default)
    {
        coreDbContext.NotificationPreferences.Update(notificationPreference);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ShouldNotifyAsync(string userId, NotificationType notificationType, CancellationToken cancellationToken = default)
    {
        return await coreDbContext.NotificationPreferences
            .AnyAsync(np => np.UserId == userId && np.NotificationType == notificationType && np.IsEnabled, cancellationToken);
    }
}