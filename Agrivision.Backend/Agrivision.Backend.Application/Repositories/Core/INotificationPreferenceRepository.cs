using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface INotificationPreferenceRepository
{
    // add 
    Task AddAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default);
    
    // get by user id
    Task<IReadOnlyList<NotificationPreference>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    
    // update
    Task UpdateAsync(NotificationPreference notificationPreference, CancellationToken cancellationToken = default);
}