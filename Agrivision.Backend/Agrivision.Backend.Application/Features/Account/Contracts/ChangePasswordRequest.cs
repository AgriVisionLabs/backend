

namespace Agrivision.Backend.Application.Features.Account.Contracts;
public record ChangePasswordRequest
(
    string CurrentPassword,
    string NewPassword
);
