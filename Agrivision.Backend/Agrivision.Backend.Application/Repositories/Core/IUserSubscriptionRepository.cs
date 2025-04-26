
using Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Repositories.Core;
public interface IUserSubscriptionRepository
{
    Task<UserSubscription?> GetByIdAsync(Guid id,CancellationToken cancellationToken);
    Task<UserSubscription?> GetByUserIdAsync(string userId, CancellationToken cancellationToken);
    Task AddAsync(UserSubscription userSubscription, CancellationToken cancellationToken);

}
