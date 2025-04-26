using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Entities.Core;
using Stripe;

namespace Agrivision.Backend.Infrastructure.Services.Payment;
public class StripeService(): IStripeService
{
    public async Task<string> CreatePaymentIntentAsync(string email, SubscriptionPlan plan)
    {
        // Create customer in Stripe
        var customerService = new CustomerService();
        var customer = await customerService.CreateAsync(new Stripe.CustomerCreateOptions
        {
            Email = email
        });

        // Create a Payment Intent
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
        {
            Amount = (long)(plan.Price * 100), // Convert to cents (e.g., 499.9 EGP = 49990)
            Currency = plan.Currency.ToLower(), // "egp"
            Customer = customer.Id,
            AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
            {
                Enabled = true
            },
            Metadata = new Dictionary<string, string>
            {
                { "plan_id", plan.Id.ToString() }
            }
        });

        return paymentIntent.ClientSecret; // Return client secret for frontend confirmation
    }

    public async Task<string> CreateSubscriptionAfterPaymentAsync(string paymentIntentId, SubscriptionPlan plan)
    {

        // Verify Payment Intent status
        var paymentIntentService = new PaymentIntentService();
        var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);
        if (paymentIntent.Status != "succeeded")
        {
            throw new Exception("Payment Intent not successful.");
        }


        // Create subscription in Stripe
        var subscriptionService = new SubscriptionService();
        var subscription = await subscriptionService.CreateAsync(new SubscriptionCreateOptions
        {
            Customer = paymentIntent.CustomerId,
            Items = new List<SubscriptionItemOptions>
            {
                new SubscriptionItemOptions
                {
                    PriceData = new SubscriptionItemPriceDataOptions
                    {
                        Currency = plan.Currency.ToLower(),
                        Product = "prod_agri_plan", // Create a product in Stripe Dashboard
                        UnitAmount = (long)(plan.Price * 100), // Convert to cents
                        Recurring = new SubscriptionItemPriceDataRecurringOptions
                        {
                            Interval = "month"
                        }
                    }
                }
            },
            PaymentBehavior = "allow_incomplete",
            PaymentSettings = new SubscriptionPaymentSettingsOptions
            {
                PaymentMethodTypes = ["card"]
            }
        });

        return subscription.Id;


    }


}
