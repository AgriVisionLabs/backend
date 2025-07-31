using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Api.Hubs;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Notifications.Commands;
using Agrivision.Backend.Application.Features.Notifications.Queries;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet("")]
    public async Task<IActionResult> GetAllNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var result = await mediator.Send(new GetNotificationsQuery(userId), cancellationToken);
        
        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
    }
    
    [HttpPost("{notificationId}/mark-read")]
    public async Task<IActionResult> MarkNotificationReadAsync([FromRoute] Guid notificationId, CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new MarkNotificationReadCommand(notificationId, userId);
        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
    }
    
    [HttpPost("clear")]
    public async Task<IActionResult> ClearNotificationsAsync(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new ClearNotificationsCommand(userId);
        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
    }
    
    [HttpPost("mark-read")]
    public async Task<IActionResult> MarkAllNotificationsReadAsync(CancellationToken cancellationToken = default)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new MarkAllNotificationsReadCommand(userId);
        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
    }
} 