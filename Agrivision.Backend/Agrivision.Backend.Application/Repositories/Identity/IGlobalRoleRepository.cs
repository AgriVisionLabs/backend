namespace Agrivision.Backend.Application.Repositories.Identity;

public interface IGlobalRoleRepository
{
    Task<IReadOnlyList<string>> GetPermissionsAsync(IList<string> roles, CancellationToken cancellationToken = default);
}