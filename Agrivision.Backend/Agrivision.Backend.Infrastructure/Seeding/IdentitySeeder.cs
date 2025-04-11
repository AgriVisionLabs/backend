using System.Security.Claims;
using Agrivision.Backend.Application.Security;
using Agrivision.Backend.Domain.Abstractions.Consts;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Infrastructure.Seeding;

public static class IdentitySeeder
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

        var adminSettings = options.Value;

        if (string.IsNullOrWhiteSpace(adminSettings.Email) || string.IsNullOrWhiteSpace(adminSettings.Password))
            throw new InvalidOperationException("Admin email or password is not configured properly.");
        
        
        var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminSettings.UserName,
                Email = adminSettings.Email,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                FirstName = adminSettings.FirstName,
                LastName = adminSettings.LastName,
                PhoneNumber = adminSettings.PhoneNumber
            };

            var result = await userManager.CreateAsync(adminUser, adminSettings.Password);
            if (!result.Succeeded)
                throw new Exception("Failed to create admin user: " +
                                    string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        else
        {
            logger.LogInformation("Admin user already seeded.");
        }

        var isInRole = await userManager.IsInRoleAsync(adminUser, adminSettings.Role);
        if (!isInRole)
            await userManager.AddToRoleAsync(adminUser, adminSettings.Role);
        
        logger.LogInformation("Admin user was seeded successfully.");
    }

    public static async Task SeedGlobalRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");
        
        var globalRoles = new List<string> { "Admin", "Support", "Member" };

        foreach (var role in globalRoles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        logger.LogInformation("Global roles were seeded successfully.");
    }

    public static async Task SeedGlobalRolePermissionAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

        var rolePermissionMap = new Dictionary<string, List<string>>
        {
            ["Admin"] = GlobalPermissionsRegistry.GetAllPermissions()!.ToList()!,
            ["Support"] =
            [
                GlobalPermissions.ReadUsers,
                GlobalPermissions.ReadGlobalRoles,
                GlobalPermissions.ViewAnyFarm,
                GlobalPermissions.ImpersonateUser
            ],
            ["Member"] = []
        };

        foreach (var (roleName, permissions) in rolePermissionMap)
        {
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                logger.LogWarning("Role '{RoleName}' not found. Skipping permission seeding.", roleName);
                continue;
            }

            var existingClaim = await roleManager.GetClaimsAsync(role);

            foreach (var permission in permissions.Where(permission => !existingClaim.Any(c => c.Type == GlobalPermissions.Type && c.Value == permission)))
            {
                await roleManager.AddClaimAsync(role, new Claim(GlobalPermissions.Type, permission));
            }
            logger.LogInformation("Seeded permissions for role '{RoleName}'.", roleName);
        }
    }
}