using System.Security.Claims;
using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Invitations.Commands;
using Agrivision.Backend.Application.Features.Farm.Contracts;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationActionsController(IMediator mediator) : ControllerBase
    {
        [HttpPost("accept")]
        public async Task<IActionResult> AcceptInvitationAsync([FromBody] AcceptInvitationRequest request, CancellationToken cancellationToken = default)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Result.Failure(TokenErrors.InvalidToken).ToProblem(TokenErrors.InvalidToken.ToStatusCode());

            var command = new AcceptInvitationCommand(userId, request.Token);
            var result = await mediator.Send(command, cancellationToken);

            return result.Succeeded ? NoContent() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
