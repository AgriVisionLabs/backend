using Agrivision.Backend.Infrastructure.Auth.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

public class FarmRoleAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
{
    private readonly AuthorizationOptions _authorizationOptions;

    public FarmRoleAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
    {
        _authorizationOptions = options.Value;
    }

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);
        if (policy != null)
            return policy;

        var allowedRoles = policyName.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToArray();

        var permissionPolicy = new AuthorizationPolicyBuilder()
            .AddRequirements(new FarmRoleRequirement(allowedRoles))
            .Build();

        _authorizationOptions.AddPolicy(policyName, permissionPolicy);
        return permissionPolicy;
    }
}