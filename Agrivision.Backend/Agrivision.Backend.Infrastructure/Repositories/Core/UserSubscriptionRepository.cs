using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class UserSubscriptionRepository(CoreDbContext coreDbContext) : IUserSubscriptionRepository
{
    public async Task<UserSubscription?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await coreDbContext.UserSubscriptions.FirstOrDefaultAsync(us => us.Id == id, cancellationToken);

    }
    public async Task<UserSubscription?> GetByUserIdAsync(string userId, CancellationToken cancellationToken)
    {
        return await coreDbContext.UserSubscriptions.FirstOrDefaultAsync(us => us.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(UserSubscription userSubscription, CancellationToken cancellationToken)
    {
        await coreDbContext.UserSubscriptions.AddAsync(userSubscription,cancellationToken);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserSubscription?> GetByStripeSubscriptionIdAsync(string stripeSubscriptionId, CancellationToken cancellationToken)
    {
        return await coreDbContext.UserSubscriptions.FirstOrDefaultAsync(us => us.StripeSubscriptionId == stripeSubscriptionId, cancellationToken);
    }

    public async Task UpdateAsync(UserSubscription subscription, CancellationToken cancellationToken)
    {
        coreDbContext.UserSubscriptions.Update(subscription);
        await coreDbContext.SaveChangesAsync(cancellationToken);
    }


}
