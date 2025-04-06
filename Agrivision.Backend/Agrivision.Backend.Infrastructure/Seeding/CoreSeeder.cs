using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Infrastructure.Seeding;

public static class CoreSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        
        var coreDbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");
        
        var defaultRoles = new List<FarmRole>
        {
            new() { Name = "Owner", Description = "Full Access to the farm", IsDefault = true },
            new() { Name = "Manager", Description = "Manages day-to-day operations", IsDefault = true },
            new() { Name = "Expert", Description = "Can view data and offer advice", IsDefault = true },
            new() { Name = "Worker", Description = "Limited access to assigned tasks", IsDefault = true }
        };

        foreach (var role in defaultRoles)
        {
            var exists = await coreDbContext.FarmRoles
                .AnyAsync(r => r.Name == role.Name && !r.IsDeleted);

            if (!exists)
            {
                coreDbContext.FarmRoles.Add(role);
            }
        }

        await coreDbContext.SaveChangesAsync();
        
        logger.LogInformation("Farm roles were seeded successfully.");
    }

    public static async Task SeedDemoFarm(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var coreDbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");

        var adminSettings = options.Value;
        
        try
        {
            // get admin user
            var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
            if (adminUser == null)
                throw new Exception("Couldn't find admin user.");

            var farmExists =
                await coreDbContext.Farms.AnyAsync(f =>
                    f.Name == "Giga Farm" && f.CreatedById == adminUser.Id && !f.IsDeleted);
            
            // check if farm already exists
            if (farmExists)
            {
                logger.LogInformation("Demo farm already seeded.");
                return;
            }

            // create new farm
            var demoFarm = new Farm
            {
                Name = "Giga Farm",
                Area = 16.25,
                Location = "Greenland",
                SoilType = SoilTypes.Clay,
                CreatedById = adminUser.Id
            };

            await coreDbContext.Farms.AddAsync(demoFarm);
            await coreDbContext.SaveChangesAsync();
            
            logger.LogInformation("Seeded demo farm: {Name}", demoFarm.Name);
            
            // add some fields
            var fields = new List<Field>
            {
                new()
                {
                    Name = "North Field",
                    Area = 8.5,
                    IsActive = true,
                    FarmId = demoFarm.Id,
                    CreatedById = adminUser.Id
                },
                new()
                {
                    Name = "South Field",
                    Area = 6.0,
                    IsActive = true,
                    FarmId = demoFarm.Id,
                    CreatedById = adminUser.Id
                }
            };

            await coreDbContext.Fields.AddRangeAsync(fields);
            await coreDbContext.SaveChangesAsync();
            
            logger.LogInformation("Seeded fields for farm: {FarmName}", demoFarm.Name);
            
            // get owner role
            var ownerRole = await coreDbContext.FarmRoles
                .FirstOrDefaultAsync(r => r.Name == "Owner" && !r.IsDeleted);

            if (ownerRole == null)
            {
                logger.LogError("Owner farm role not found.");
                throw new Exception("Owner farm role not found.");
            }
            
            // assign admin as owner
            var farmUserRole = new FarmUserRole
            {
                FarmId = demoFarm.Id,
                UserId = adminUser.Id,
                FarmRoleId = ownerRole.Id,
                IsActive = true,
                AcceptedAt = DateTime.Now,
                CreatedById = adminUser.Id
            };

            await coreDbContext.FarmUserRoles.AddAsync(farmUserRole);
            await coreDbContext.SaveChangesAsync();
            
            logger.LogInformation("Admin assigned as Owner for farm: {FarmName}", demoFarm.Name);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to seed demo farm.");
        }
        
    }
}