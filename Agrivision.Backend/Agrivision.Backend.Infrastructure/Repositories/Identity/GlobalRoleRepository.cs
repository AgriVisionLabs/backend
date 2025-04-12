using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions.Consts;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Infrastructure.Repositories.Identity;

public class GlobalRoleRepository(ApplicationUserDbContext applicationUserDbContext) : IGlobalRoleRepository
{
    public async Task<IReadOnlyList<string>> GetPermissionsAsync(IList<string> roles, CancellationToken cancellationToken = default)
    {
        var userPermissions = await applicationUserDbContext.Roles
            .Where(r => roles.Contains(r.Name!))
            .Join(
                applicationUserDbContext.RoleClaims,
                role => role.Id,
                claim => claim.RoleId,
                (role, claim) => new { claim.ClaimType, claim.ClaimValue }
            )
            .Where(x => x.ClaimType == GlobalPermissions.Type) // filter only permission claims
            .Select(x => x.ClaimValue!)
            .Distinct()
            .ToListAsync(cancellationToken);

        return userPermissions;
    }
}