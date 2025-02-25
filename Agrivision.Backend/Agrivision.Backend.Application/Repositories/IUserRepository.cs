using Agrivision.Backend.Domain.Interfaces;

namespace Agrivision.Backend.Application.Repositories;

public interface IUserRepository
{
    Task<IApplicationUser?> FindByEmailAsync(string email);
    Task<IApplicationUser?> FindByUserNameAsync(string userName);
    Task<IApplicationUser?> FindByIdAsync(string userId);
    Task<bool> CheckPasswordAsync(IApplicationUser user, string password);
    Task UpdateAsync(IApplicationUser user);
    Task<bool> CreateUserAsync(IApplicationUser user, string password);
    Task<string> GenerateEmailConfirmationTokenAsync(IApplicationUser user);
    Task<bool> ConfirmEmailAsync(IApplicationUser user, string code);
}