using Agrivision.Backend.Api.Extensions;
using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request, CancellationToken cancellationToken = default)
        {
            var result = await authService.GetTokenAsync(request, cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var result = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.Succeeded ? Ok(result.Value) : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await authService.RegisterAsync(request, baseUrl,cancellationToken);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
        {
            var result = await authService.ConfirmEmailAsync(request);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }

        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfrimaionEmail([FromBody] ResendConfirmationEmailRequest request)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = await authService.ResendConfirmationEmailAsync(request, baseUrl);
            return result.Succeeded ? Ok() : result.ToProblem(result.Error.ToStatusCode());
        }
    }
}
