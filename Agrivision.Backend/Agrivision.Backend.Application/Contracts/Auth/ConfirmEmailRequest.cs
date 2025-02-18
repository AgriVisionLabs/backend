namespace Agrivision.Backend.Application.Contracts.Auth;

public record ConfirmEmailRequest
(
    string UserId,
    string Code
);