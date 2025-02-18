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
        if (user is ApplicationUser applicationUser)
        {
            await userManager.UpdateAsync(applicationUser);
        }
    }
    
    public async Task<bool> CreateUserAsync(IApplicationUser user, string password)
    {
        var applicationUser = new ApplicationUser // this works while mapping using mapster doesn't because doing this creates a new ApplicationUser and ef automatically generates a new id for it meanwhile if we used mapster it won't create an id 
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