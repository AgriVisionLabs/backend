using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Application.Features.Auth.Contracts;
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
        public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new AuthQuery(request.Email, request.Password), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new RefreshTokenCommand(request.Token, request.RefreshToken), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new RegisterCommand(request.UserName, request.Email, request.Password, request.FirstName, request.LastName, request.PhoneNumber), cancellationToken);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await mediator.Send(new ConfirmEmailCommand(request.Token));
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            var result = await mediator.Send(new ResendConfirmationEmailCommand(request.Email));
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordResetAsync([FromBody] RequestPasswordResetRequest request, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new RequestPasswordResetCommand(request.Email), cancellationToken);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("verify-password-reset-otp")]
        public async Task<IActionResult> VerifyPasswordResetAsync([FromBody] VerifyPasswordResetOtpRequest request, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new VerifyPasswordResetOtpCommand(request.Email, request.OtpCode), cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("password-reset")]
        public async Task<IActionResult> PasswordResetAsync([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken = default)
        {
            var result = await mediator.Send(new ResetPasswordCommand(request.Token, request.NewPassword), cancellationToken);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
