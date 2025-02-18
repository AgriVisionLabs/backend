using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Entities;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Repositories;

public class IdentityRepository (UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<IApplicationUser?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }

    public async Task<IApplicationUser?> FindByIdAsync(string userId)
    {
        return await userManager.FindByIdAsync(userId);
    }

    public async Task<bool> CheckPasswordAsync(IApplicationUser user, string password)
    {
        if (user is not ApplicationUser appUser)
            return false;

        return await userManager.CheckPasswordAsync(appUser, password);
    }

    public async Task UpdateAsync(IApplicationUser user)
    {
        await userManager.UpdateAsync(user.Adapt<ApplicationUser>());
    }
    
    public async Task<bool> CreateUserAsync(IApplicationUser user, string password)
    {
        var applicationUser = new ApplicationUser
        {
            UserName = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
        
        var result = await userManager.CreateAsync(applicationUser, password);
        return result.Succeeded;
    }
}