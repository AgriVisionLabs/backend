using Agrivision.Backend.Application.Abstractions;
using Agrivision.Backend.Application.Contracts.Auth;

namespace Agrivision.Backend.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(AuthRequest authRequest, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);

    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
}