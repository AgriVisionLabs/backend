using Agrivision.Backend.Application.Enums;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Interfaces;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Repositories;

public class AuthRepository(SignInManager<ApplicationUser> signInManager) : IAuthRepository
{
    public async Task<SignInStatus> PasswordSignInAsync(IApplicationUser user, string password)
    {
        if (user is not ApplicationUser applicationUser)
            throw new Exception("Cannot use PasswordSignInAsync with non ApplicationUser types");
        
        var result = await signInManager.PasswordSignInAsync(applicationUser, password, false, false);
        
        if (result.Succeeded) return SignInStatus.Success;
        
        return result.IsNotAllowed ? SignInStatus.EmailNotConfirmed : SignInStatus.InvalidCredentials;
    }
}