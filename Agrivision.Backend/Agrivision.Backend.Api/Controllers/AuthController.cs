using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Agrivision.Backend.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("")]
        public async Task<IActionResult> LoginAsync([FromBody] AuthRequest request, CancellationToken cancellationToken = default)
        {
            var authRes = await authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return authRes is null ? BadRequest("Invalid Credentials") : Ok(authRes);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var authRes = await authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);
            return authRes is null ? BadRequest("Invalid Token") : Ok(authRes);
        }
    }
}
