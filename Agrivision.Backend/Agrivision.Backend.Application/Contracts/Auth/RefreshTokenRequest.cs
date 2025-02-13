namespace Agrivision.Backend.Application.Contracts.Auth;

public record RefreshTokenRequest
(
    string Token,
    string RefreshToken
);