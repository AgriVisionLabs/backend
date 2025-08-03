namespace Agrivision.Backend.Application.Features.Account.Contracts;
public record UserProfileResponse
(
   string Email,
   string UserName,
   string PfpImageUrl,
   string FirstName,
   string LastName,
   string? PhoneNumber
);
