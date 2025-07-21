using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IClearedNotificationRepository
{
    // add
    Task AddClearedNotificationAsync(ClearedNotification clearedNotification, CancellationToken cancellationToken = default);
}