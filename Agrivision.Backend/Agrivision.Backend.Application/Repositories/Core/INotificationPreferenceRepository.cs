using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface INotificationPreferenceRepository
{
    // add 
    Task AddAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default);
    
    // get by user id
    Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    
    // update
    Task UpdateAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default);
    
    // should notify
    Task<bool> ShouldNotifyAsync(string userId, NotificationType notificationType, CancellationToken cancellationToken = default);
}