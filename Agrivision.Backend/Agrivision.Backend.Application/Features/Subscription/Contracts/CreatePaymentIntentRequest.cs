

namespace Agrivision.Backend.Application.Features.Subscription.Contracts;
public record CreatePaymentIntentRequest
(
    string UserId,
    Guid PlanId
);