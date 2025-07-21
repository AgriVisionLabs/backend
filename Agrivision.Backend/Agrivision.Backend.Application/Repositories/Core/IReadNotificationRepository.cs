using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface IReadNotificationRepository
{
    // add
    Task AddReadNotificationAsync(ReadNotification readNotification, CancellationToken cancellationToken = default);
    
    // get all
    Task<IReadOnlyList<ReadNotification>> GetAllReadNotificationsAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
    
    // get by id
    Task<ReadNotification?> GetReadNotificationByIdAsync(Guid notificationId, string userId, CancellationToken cancellationToken = default);
}