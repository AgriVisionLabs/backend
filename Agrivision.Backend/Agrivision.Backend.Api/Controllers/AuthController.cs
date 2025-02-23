using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthQuery query, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(query, cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand command,
            CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailCommand command)
        {
            var result = await mediator.Send(command);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfrimaionEmail([FromBody] ResendConfirmationEmailCommand command)
        {
            var result = await mediator.Send(command);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
