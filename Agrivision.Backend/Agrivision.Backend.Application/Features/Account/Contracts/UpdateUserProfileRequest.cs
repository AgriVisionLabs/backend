namespace Agrivision.Backend.Application.Features.Account.Contracts;
public record UpdateUserProfileRequest
(
    string FirstName,
    string LastName,
    string UserName,
    string? PhoneNumber
);

