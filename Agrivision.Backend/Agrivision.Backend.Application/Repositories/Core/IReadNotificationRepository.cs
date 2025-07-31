using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IReadNotificationRepository
{
    // add
    Task AddAsync(ReadNotification readNotification, CancellationToken cancellationToken = default);
    
    // get all
    Task<IReadOnlyList<ReadNotification>> GetAllReadNotificationsAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);

    Task<HashSet<Guid>> GetReadNotificationIdsByUserAsync(string userId, CancellationToken cancellationToken = default);
    
    // get by id
    Task<ReadNotification?> GetReadNotificationByIdAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
    
    // is read
    Task<bool> IsNotificationReadAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
    
    // add range async
    Task AddRangeAsync(IEnumerable<ReadNotification> readNotifications, CancellationToken cancellationToken = default);
}