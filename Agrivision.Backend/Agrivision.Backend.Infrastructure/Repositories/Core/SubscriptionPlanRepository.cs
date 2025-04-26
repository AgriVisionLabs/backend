
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Core;
public class SubscriptionPlanRepository(CoreDbContext coreDbContext) : ISubscriptionPlanRepository
{
    public async Task<SubscriptionPlan?> GetByIdAsync(Guid planId, CancellationToken cancellationToken)
    {
        return await coreDbContext.SubscriptionPlans.FirstOrDefaultAsync(sp => sp.Id == planId,cancellationToken);
    }

}
