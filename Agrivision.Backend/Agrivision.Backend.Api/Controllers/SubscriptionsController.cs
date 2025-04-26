using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Api.Extensions;
namespace Agrivision.Backend.Api.Controllers;
[Route("[controller]")]
[ApiController]
public class SubscriptionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("create-payment-intent")]
    public async Task<IActionResult> CreatePaymentIntent([FromBody] CreatePaymentIntentRequest request)
    {
        var command = new CreatePaymentIntentCommand(request.UserId,request.PlanId);
        var result = await mediator.Send(command);
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());

    }

    [HttpPost("confirm-subscription")]
    public async Task<IActionResult> ConfirmSubscription([FromBody] ConfirmSubscriptionRequest request)
    {
        var command = new ConfirmSubscriptionCommand(request.UserId, request.PlanId,request.PaymentIntentId);

        var result = await mediator.Send(command);
        return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());



    }


}
