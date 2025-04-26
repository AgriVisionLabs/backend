

namespace Agrivision.Backend.Application.Features.Subscription.Contracts;
public record ConfirmSubscriptionRequest
    (

    string UserId,
    Guid PlanId,
    string PaymentIntentId

    ) ;