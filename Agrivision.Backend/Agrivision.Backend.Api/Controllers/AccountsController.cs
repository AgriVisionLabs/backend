using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using Agrivision.Backend.Application.Features.Account.Queries;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Features.Account.Contracts;
using Agrivision.Backend.Application.Features.Account.Commands;


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

    [HttpPut("change-password")]
    public async Task<IActionResult>  ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var command =new ChangePasswordCommand ( userId!,request.CurrentPassword,request.NewPassword);

        var result = await mediator.Send(command, cancellationToken);

        return result.Succeeded ? Ok(result) : result.ToProblem(result.Error.ToStatusCode());

    }
}
