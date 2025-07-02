using Agrivision.Backend.Domain.Entities.Shared;
using Agrivision.Backend.Domain.Enums.Core;

namespace Agrivision.Backend.Domain.Entities.Core;

public class UserSubscription : AuditableEntity
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty; 
    public Guid SubscriptionPlanId { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public UserSubscriptionStatus Status { get; set; }= UserSubscriptionStatus.Active;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string StripeSubscriptionId { get; set; } = string.Empty; 
}
