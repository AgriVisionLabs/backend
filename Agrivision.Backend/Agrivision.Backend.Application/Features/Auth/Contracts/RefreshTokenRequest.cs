namespace Agrivision.Backend.Application.Features.Auth.Contracts;

public record RefreshTokenRequest
(
    string Token,
    string RefreshToken
);