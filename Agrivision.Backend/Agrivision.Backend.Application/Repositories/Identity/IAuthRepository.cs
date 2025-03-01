using Agrivision.Backend.Domain.Enums.Identity;
using Agrivision.Backend.Domain.Interfaces.Identity;

namespace Agrivision.Backend.Application.Repositories;

public interface IAuthRepository
{
    Task<SignInStatus> PasswordSignInAsync(IApplicationUser user, string password); // returns SignInStatus to be able to give a proper error; you may ask why not just return a result with the error -> answer is we can't and shouldn't since Errors define business logic that's why we used an enum which is just a variable with no logic
}