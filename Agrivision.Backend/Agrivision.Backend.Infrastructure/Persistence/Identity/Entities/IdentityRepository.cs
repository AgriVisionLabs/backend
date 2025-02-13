using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;

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
}