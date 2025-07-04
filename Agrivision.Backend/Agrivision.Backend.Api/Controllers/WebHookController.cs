using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Domain.Enums.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace Agrivision.Backend.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class WebHookController(IMediator mediator, IStripeService stripeService, ILogger<WebHookController> logger) : ControllerBase
{
    [HttpPost("stripe-webhook")]
    public async Task<IActionResult> HandleWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
      
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                Request.Headers["Stripe-Signature"],
                await stripeService.GetWebhookSecret()
            );

            switch (stripeEvent.Type)
            {
                case EventTypes.CheckoutSessionCompleted:
                    var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                    
                    if (session?.Mode == "subscription")
                    {
                        Guid? planId= session.Metadata.TryGetValue("PlanId", out var planIdValue)
                                      && Guid.TryParse(planIdValue, out var parsedGuid)
                                      ? parsedGuid
                                      : null;
                        var command = new ConfirmSubscriptionCommand(session.CustomerEmail, session.SubscriptionId, planId);
                        var result = await mediator.Send(command);
                        return result.Succeeded? Ok()
                                               : result.ToProblem(result.Error.ToStatusCode());
                    }
                break;

                case EventTypes.InvoicePaymentFailed:
                    var invoice = stripeEvent.Data.Object as Stripe.Invoice;
                    if (invoice != null)
                    {
                        var SubscriptionId = invoice.Customer.Subscriptions.FirstOrDefault()?.Id;
                        var updateSubscriptionStatusCommand = new UpdateSubscriptionStatusCommand(SubscriptionId!,UserSubscriptionStatus.Expired);
                        var result = await mediator.Send(updateSubscriptionStatusCommand);
                        return result.Succeeded ? Ok()
                        : result.ToProblem(result.Error.ToStatusCode());
                    }
                break;

                default:
                    
                    logger.LogInformation("Unhandled Stripe event type: {EventType}", stripeEvent.Type);
                    break;

            }
        }
        catch (StripeException ex)
        {
            
            logger.LogError(ex, "Stripe webhook error");
            return BadRequest();
        }
        return Ok();

    }
}
