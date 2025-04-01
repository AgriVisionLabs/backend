using Microsoft.AspNetCore.Authorization;


namespace Agrivision.Backend.Infrastructure.Auth.Filters;
public class FarmRoleAttribute : AuthorizeAttribute
{
    public FarmRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles); 
    }
}
