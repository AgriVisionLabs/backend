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
            var res = await authService.GetTokenAsync(request, cancellationToken);
            return res.Succeeded ? Ok(res.Value) : res.ToProblem(res.Error.ToStatusCode());
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var res = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return res.Succeeded ? Ok(res.Value) : res.ToProblem(res.Error.ToStatusCode());
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request,
            CancellationToken cancellationToken = default)
        {
            var res = await authService.RegisterAsync(request, cancellationToken);
            if (true)
                return res.Succeeded ? Ok(res.Value) : res.ToProblem(res.Error.ToStatusCode());
        }
    }
}
