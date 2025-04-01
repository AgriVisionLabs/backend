

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Infrastructure.Auth.Filters;
public class FarmRoleHandler : AuthorizationHandler<FarmRoleRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FarmRoleRequirement requirement)
    {
        if (!context.User.Identity?.IsAuthenticated ?? false)
        {
            context.Fail();
            return ;
        }

        // Check if the user is an admin (bypasses farm-specific role check)
        var isAdmin = context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "admin");
        if (isAdmin)
        {
            context.Succeed(requirement);
            return ;
        }
        var farmId = GetFarmIdFromContext(context);
        var userHasRole = context.User.HasClaim(c =>
            c.Type == $"Farm:{farmId}:Role" && requirement.AllowedRoles.Contains(c.Value));

        if (userHasRole)
            context.Succeed(requirement);
        else
            context.Fail();

        return ;
    }

    private int GetFarmIdFromContext(AuthorizationHandlerContext context)
    {
        var httpContext = context.Resource as HttpContext;
        var farmIdStr = httpContext?.Request.RouteValues["farmId"]?.ToString();
        return int.TryParse(farmIdStr, out var farmId) ? farmId : 0; // Default or throw if invalid
    }
}