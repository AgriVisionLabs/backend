using Agrivision.Backend.Domain.Entities.Core;
namespace Agrivision.Backend.Application.Repositories.Core;
public interface ISubscriptionPlanRepository
{
    Task<SubscriptionPlan?> GetByIdAsync(Guid planId, CancellationToken cancellationToken);
    //Task<List<SubscriptionPlan>> GetAllActiveAsync(CancellationToken cancellationToken);
}
