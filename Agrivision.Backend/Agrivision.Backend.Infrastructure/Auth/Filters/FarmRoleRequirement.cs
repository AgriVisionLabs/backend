

using Microsoft.AspNetCore.Authorization;

namespace Agrivision.Backend.Infrastructure.Auth.Filters;
public class FarmRoleRequirement : IAuthorizationRequirement
{
    public string[] AllowedRoles { get; }

    public FarmRoleRequirement(string[] allowedRoles)
    {
        AllowedRoles = allowedRoles;
    }
}
