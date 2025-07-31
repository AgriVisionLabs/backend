using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;

public interface INotificationRepository
{
    // get all
    Task<IReadOnlyList<Notification>> GetAllNotificationsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    
    // get by id
    Task<Notification?> GetByIdAsync(Guid id, string userId, CancellationToken cancellationToken = default);
    
    // add 
    Task AddAsync(Notification notification, CancellationToken cancellationToken = default);
}