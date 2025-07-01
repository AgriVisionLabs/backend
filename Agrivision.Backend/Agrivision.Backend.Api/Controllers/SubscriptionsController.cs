using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Agrivision.Backend.Api.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class SubscriptionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("create-Subscription")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
    {
       
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var command = new CreateSubscriptionCommand(userId!,request.PlanId);
        var result = await mediator.Send(command);
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
    }
    [AllowAnonymous]
    [HttpGet("confirm-subscription")]
    public async Task<IActionResult> ConfirmSubscription([FromQuery] string session_Id)
    {
        var command = new ConfirmSubscriptionCommand(session_Id);

        var result = await mediator.Send(command);
        return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
    }
}
