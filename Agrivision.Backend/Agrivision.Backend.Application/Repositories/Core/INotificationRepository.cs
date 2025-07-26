using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface INotificationRepository
{
    // get all
    Task<IReadOnlyList<Notification>> GetAllNotificationsAsync(string userId, CancellationToken cancellationToken = default);
    
    // add 
    Task AddNotificationAsync(Notification notification, CancellationToken cancellationToken = default);
}