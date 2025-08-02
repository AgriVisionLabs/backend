using Agrivision.Backend.Application.Features.Subscription.Contracts;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Agrivision.Backend.Application.Features.Subscription.Commands;
using Agrivision.Backend.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Subscription;
using Agrivision.Backend.Application.Features.Subscription.Queries;
using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Api.Controllers;
[Route("[controller]")]
[ApiController]
[Authorize]
public class SubscriptionsController(IMediator mediator) : ControllerBase
{
    [HttpPost("")]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new CreateSubscriptionCommand(userId!,request.PlanId);
        var result = await mediator.Send(command, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
    }

    [HttpGet("")]
    public async Task<IActionResult> GetUserSubscription(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());
        
        var query = new GetUserSubscriptionPlanQuery(userId);
        var result = await mediator.Send(query, cancellationToken);
        
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
    }
    
    [HttpPost("cancel")]
    public async Task<IActionResult> CancelSubscription(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new CancelSubscriptionCommand(userId);
        var result = await mediator.Send(command, cancellationToken);
        
        return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
    }
}
