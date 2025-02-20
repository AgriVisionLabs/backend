using Agrivision.Backend.Application.Contracts.Auth;
using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Services.Auth;

public interface IAuthService
{
    Task<Result<AuthResponse>> GetTokenAsync(AuthRequest authRequest, CancellationToken cancellationToken = default);

    Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken,
        CancellationToken cancellationToken = default);
    Task<Result> RegisterAsync(RegisterRequest request, string baseUrl, CancellationToken cancellationToken = default);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request);
    Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request, string baseUrl);
}