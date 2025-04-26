using  Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Services.Payment;
public interface IStripeService
{
    Task<string> CreatePaymentIntentAsync(string email, SubscriptionPlan plan);
    Task<string> CreateSubscriptionAfterPaymentAsync(string paymentIntentId, SubscriptionPlan plan);

}
