using  Agrivision.Backend.Domain.Entities.Core;

namespace Agrivision.Backend.Application.Services.Payment;
public interface IStripeService
{
    Task<string> CreateSubscriptionCheckoutSessionAsync(string email, SubscriptionPlan plan);
   // Task<string> GetCustomerEmailAsync(string sessionId);
   // Task<string?> GetSubscriptionIdAsync(string sessionId);
   // Task<Guid?> GetPlanIdAsync(string sessionId);
    Task<string> GetWebhookSecret();
}
