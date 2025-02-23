using System.Text;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Domain.Interfaces;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Agrivision.Backend.Infrastructure.Repositories;

public class UserRepository (UserManager<ApplicationUser> userManager) : IUserRepository
{
    public async Task<IApplicationUser?> FindByEmailAsync(string email)
    {
        return await userManager.FindByEmailAsync(email);
    }
    
    public async Task<IApplicationUser?> FindByUserNameAsync(string userName)
    {
        return await userManager.FindByNameAsync(userName);
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
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber
        };
        var result = await userManager.CreateAsync(applicationUser, password);
        return result.Succeeded;
    }
    
    public async Task<bool> ConfirmEmailAsync(IApplicationUser user, string code)
    {
        if (user is ApplicationUser applicationUser)
        {
            var result = await userManager.ConfirmEmailAsync(applicationUser, code);

            return result.Succeeded;
        }

        throw new Exception("Can't use ConfirmEmailAsync with non ApplicationUser type objects");
    }
}