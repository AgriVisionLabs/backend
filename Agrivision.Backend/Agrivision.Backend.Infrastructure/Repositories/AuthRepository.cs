using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Enums;
using Agrivision.Backend.Domain.Interfaces;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Repositories;

public class AuthRepository(SignInManager<ApplicationUser> signInManager) : IAuthRepository // currently not used though because when used with login it will tell the user the email is not verified even though the password is incorrect 
{
    public async Task<SignInStatus> PasswordSignInAsync(IApplicationUser user, string password)
    {
        if (user is not ApplicationUser applicationUser)
            throw new Exception("Cannot use PasswordSignInAsync with non ApplicationUser types");
        
        var result = await signInManager.PasswordSignInAsync(applicationUser, password, false, false);

        if (result.Succeeded) 
            return SignInStatus.Success;

        if (result.IsLockedOut) 
            return SignInStatus.LockedOut;

        if (result.IsNotAllowed) 
            return SignInStatus.EmailNotConfirmed; // This will only be checked after confirming credentials

        return SignInStatus.InvalidCredentials; // we do this because we can't return signin result because it is an identity member 
    }
}