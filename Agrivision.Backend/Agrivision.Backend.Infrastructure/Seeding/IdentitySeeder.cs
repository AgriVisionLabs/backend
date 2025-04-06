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
        using var scope = serviceProvider.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");

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
        using var scope = serviceProvider.CreateScope();
        
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("IdentitySeeder");
        
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
}