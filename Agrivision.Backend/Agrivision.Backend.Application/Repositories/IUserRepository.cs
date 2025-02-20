using Agrivision.Backend.Domain.Interfaces;

namespace Agrivision.Backend.Application.Repositories;

public interface IUserRepository
{
    Task<IApplicationUser?> FindByEmailAsync(string email);
    Task<IApplicationUser?> FindByIdAsync(string userId);
    Task<bool> CheckPasswordAsync(IApplicationUser user, string password);
    Task UpdateAsync(IApplicationUser user);
    Task<bool> CreateUserAsync(IApplicationUser user, string password);
    Task<string> GenerateEmailConfirmationTokenInLinkAsync(IApplicationUser user);
    bool TryDecodeConfirmationToken(string code, out string decodedCode);
    Task<bool> ConfirmEmailAsync(IApplicationUser user, string code);
    bool TryDecodeUserId(string userId, out string decodedUserId);
    string EncodeUserId(string userId);
}