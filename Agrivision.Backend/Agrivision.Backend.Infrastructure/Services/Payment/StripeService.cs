using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace Agrivision.Backend.Infrastructure.Services.Payment;
public class StripeService : IStripeService
{
    private readonly StripeClient _client;
    private readonly StripeSettings _stripeSettings;

    public StripeService(IOptions<StripeSettings> stripeSettings)
    {
        StripeConfiguration.ApiKey = stripeSettings.Value.SecretKey;
        _client = new StripeClient(stripeSettings.Value.SecretKey);
        _stripeSettings = stripeSettings.Value;
    }
    public async Task<string> CreateSubscriptionCheckoutSessionAsync(string email, SubscriptionPlan plan)
    {

        // Create Checkout Session in subscription mode
        var options = new SessionCreateOptions
        {
            CustomerEmail = email,
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "subscription",
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = plan.Currency.ToLower(),
                        Product =plan.ProductId, 
                        UnitAmount = (long)(plan.Price * 100),
                        Recurring = new SessionLineItemPriceDataRecurringOptions
                        {
                            Interval = "month"
                        }
                    },
                    Quantity = 1
                }
            },
            Metadata = new Dictionary<string, string>
            {
                   { "PlanId", plan.Id.ToString() }
            },
            SuccessUrl = $"{_stripeSettings.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = "https://.sssm/payment/cancel"
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);
        return session.Url; // Return this URL to frontend to redirect the user
    }
   /* public async Task<string> GetCustomerEmailAsync(string sessionId)
    {
        var service = new global::Stripe.Checkout.SessionService(_client);

        var session = await service.GetAsync(sessionId);

        if (session is null)
            throw new Exception("Can not find CheckOut session with given Id.");

        if (!string.IsNullOrEmpty(session.CustomerEmail))
        {
            return session.CustomerEmail;
        }
        throw new Exception("No email found for this session.");


    }*/
   /* public async Task<string?> GetSubscriptionIdAsync(string sessionId)
    {
        var service = new global::Stripe.Checkout.SessionService(_client);

        var session = await service.GetAsync(sessionId);

        if (session is null)
            throw new Exception("Can not find CheckOut session with given Id.");

        var subscriptionId = session.SubscriptionId;



        return (subscriptionId);
    }
   */
   /* public async Task<Guid?> GetPlanIdAsync(string sessionId)
    {
        var service = new global::Stripe.Checkout.SessionService(_client);

        var session = await service.GetAsync(sessionId);

        if (session is null)
            throw new Exception("Can not find CheckOut session with given Id.");

        Guid? planId = session.Metadata.TryGetValue("PlanId", out var planIdValue)
                      && Guid.TryParse(planIdValue, out var parsedGuid)
                      ? parsedGuid
                      : null;

        return planId;
    }*/
    public async Task<string> GetWebhookSecret()
        =>_stripeSettings.WebhookSecret;
    
}
