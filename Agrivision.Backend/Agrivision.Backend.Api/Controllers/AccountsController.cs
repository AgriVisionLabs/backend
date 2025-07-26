using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using Agrivision.Backend.Application.Features.Account.Queries;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Domain.Abstractions;


namespace Agrivision.Backend.Api.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]

public class AccountsController(IMediator mediator, ILogger<AccountsController> logger) : ControllerBase
{
    [HttpGet("")] 
    public async Task<IActionResult> Info(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var result =await mediator.Send(new GetUserProfileQuery(userId!), cancellationToken);

        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());

    }
    [HttpPut("")]
    public async Task<IActionResult> Info(UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await mediator.Send(new UpdateUserProfileCommand
                                             (userId!,request.FirstName, request.LastName, request.UserName, request.PhoneNumber), cancellationToken);

        return result.Succeeded ? Ok(result) : result.ToProblem(result.Error.ToStatusCode());

    }

    [HttpPut("change-password")]
    public async Task<IActionResult>  ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var command =new ChangePasswordCommand ( userId!,request.CurrentPassword,request.NewPassword);

        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? Ok(result) : result.ToProblem(result.Error.ToStatusCode());

    }

    [HttpPut("notification-preferences")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] UpdateNotificationPreferenceRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var command = new UpdateNotificationPreferenceCommand(
            RequesterId: userId!,
            IsEnabled: request.IsEnabled,
            Irrigation: request.Irrigation,
            Task: request.Task,
            Message: request.Message,
            Alert: request.Alert,
            Warning: request.Warning,
            SystemUpdate: request.SystemUpdate
        );

        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
    }

    [HttpGet("notification-preferences")]
    public async Task<IActionResult> GetNotificationPreferences(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

        var query = new GetNotificationPreferenceQuery(userId!);
        var result = await mediator.Send(query, cancellationToken);

        return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
    }
}
