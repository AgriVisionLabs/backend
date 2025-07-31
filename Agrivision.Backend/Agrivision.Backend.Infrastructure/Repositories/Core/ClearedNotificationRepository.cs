using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;

public class ClearedNotificationRepository(CoreDbContext coreDbContext) : IClearedNotificationRepository
{
    public async Task AddAsync(ClearedNotification clearedNotification, CancellationToken cancellationToken = default)
    {
        await coreDbContext.ClearedNotifications.AddAsync(clearedNotification, cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }
}