using Agrivision.Backend.Application.Abstractions;
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
            var res = await authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return res.IsSuccess ? Ok(res.Value) : res.ToProblem(StatusCodes.Status401Unauthorized);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var res = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return res.IsSuccess ? Ok(res.Value) : res.ToProblem(StatusCodes.Status401Unauthorized);
        }
    }
}
