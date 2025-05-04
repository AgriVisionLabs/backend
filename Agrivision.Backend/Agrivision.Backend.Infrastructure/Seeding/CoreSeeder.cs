using Agrivision.Backend.Application.Security;
using Agrivision.Backend.Domain.Abstractions.Consts;
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
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");
        
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

    public static async Task SeedDemoFarmAsync(IServiceProvider serviceProvider)
    {
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");

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

    public static async Task SeedCoreRolePermissionAsync(IServiceProvider serviceProvider)
    {
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");

        var rolePermissionMap = new Dictionary<string, List<string>>
        {
            ["Owner"] = CorePermissionsRegistry.GetAllPermissions().ToList()!,
            ["Manager"] =
            [
                CorePermissions.ViewFarms,
                CorePermissions.ViewFields,
                CorePermissions.ViewFarmMembers,
                CorePermissions.AssignFarmRoles
            ],
            ["Expert"] =
            [
                CorePermissions.ViewFarms,
                CorePermissions.ViewFields
            ],
            ["Worker"] = 
            [
                CorePermissions.ViewFarms,
                CorePermissions.ViewFields,
                CorePermissions.ViewFarmMembers
            ]
        };

        foreach (var (roleName, permissions) in rolePermissionMap)
        {
            var role = await coreDbContext.FarmRoles
                .FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);

            if (role == null)
            {
                logger.LogWarning("Role not found {RoleName}", roleName);
                continue;
            }

            var existingClaims = await coreDbContext.FarmRoleClaims
                .Where(c => c.FarmRoleId == role.Id)
                .Select(c => c.ClaimValue)
                .ToListAsync();

            var newClaims = permissions
                .Where(p => !existingClaims.Contains(p))
                .Select(p => new FarmRoleClaim
                {
                    FarmRoleId = role.Id,
                    ClaimType = CorePermissions.Type,
                    ClaimValue = p
                });
            
            coreDbContext.FarmRoleClaims.AddRange(newClaims);
        }

        await coreDbContext.SaveChangesAsync();
        logger.LogInformation("Core role permissions seeded successfully.");
    }

    public static async Task SeedDevicesAsync(IServiceProvider serviceProvider)
    {
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CoreSeeder");

        var adminSettings = options.Value;

        var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
        if (adminUser is null)
            throw new Exception("Admin user not found. Who is in charge here?!");

        // Helper
        async Task<Guid> SeedSensorDevice(string serial, string mac, string key)
        {
            if (!await coreDbContext.SensorUnitDevices.AnyAsync(d => d.SerialNumber == serial || d.MacAddress == mac))
            {
                var deviceId = Guid.NewGuid();
                var device = new SensorUnitDevice
                {
                    Id = deviceId,
                    SerialNumber = serial,
                    MacAddress = mac,
                    FirmwareVersion = "1.0.0-sensor",
                    ProvisioningKey = key,
                    ManufacturedOn = DateTime.UtcNow,
                    IsAssigned = false,
                    CreatedById = adminUser.Id,
                    CreatedOn = DateTime.UtcNow
                };

                await coreDbContext.SensorUnitDevices.AddAsync(device);

                var configs = new[]
                {
                    new SensorConfiguration
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = deviceId,
                        Type = SensorType.Temperature,
                        Pin = "GPIO15",
                        IsActive = true,
                        CreatedById = adminUser.Id,
                        CreatedOn = DateTime.UtcNow
                    },
                    new SensorConfiguration
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = deviceId,
                        Type = SensorType.Humidity,
                        Pin = "GPIO15", // same pin, same sensor
                        IsActive = true,
                        CreatedById = adminUser.Id,
                        CreatedOn = DateTime.UtcNow
                    },
                    new SensorConfiguration
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = deviceId,
                        Type = SensorType.Moisture,
                        Pin = "GPIO35",
                        IsActive = true,
                        CreatedById = adminUser.Id,
                        CreatedOn = DateTime.UtcNow
                    },
                    new SensorConfiguration
                    {
                        Id = Guid.NewGuid(),
                        DeviceId = deviceId,
                        Type = SensorType.Camera,
                        Pin = "CAM0",
                        IsActive = true,
                        CreatedById = adminUser.Id,
                        CreatedOn = DateTime.UtcNow
                    }
                };

                await coreDbContext.SensorConfigurations.AddRangeAsync(configs);

                logger.LogInformation("Seeded sensor device {SerialNumber} with 3 sensor configs.", serial);

                return deviceId;
            }

            return Guid.Empty;
        }

        async Task SeedIrrigationDevice(string serial, string mac, string key)
        {
            if (!await coreDbContext.IrrigationUnitDevices.AnyAsync(d => d.SerialNumber == serial || d.MacAddress == mac))
            {
                var device = new IrrigationUnitDevice
                {
                    Id = Guid.NewGuid(),
                    SerialNumber = serial,
                    MacAddress = mac,
                    FirmwareVersion = "1.0.0-irrigate",
                    ProvisioningKey = key,
                    ManufacturedOn = DateTime.UtcNow,
                    IsAssigned = false,
                    CreatedById = adminUser.Id,
                    CreatedOn = DateTime.UtcNow
                };

                await coreDbContext.IrrigationUnitDevices.AddAsync(device);
                logger.LogInformation("Seeded irrigation device {SerialNumber}.", serial);
            }
        }

        // Production devices
        await SeedIrrigationDevice("IRRIGATE-ESP32-01", "08:A6:F7:A8:56:A4", "IRRIGATE-X7Q4-PUMP-KEY9-Z1B3L");
        await SeedSensorDevice("SENSOR-ESP32-01", "08:A6:F7:A8:32:F0", "SENSOR-X7Q4-MOIST-KEY9-Z3C4U");

        // Test devices
        await SeedIrrigationDevice("IRRIGATE-ESP32-TEST", "08:A6:F7:A8:99:99", "IRRIGATE-TEST-PUMP-KEY9-ZZZZZ");
        await SeedSensorDevice("SENSOR-ESP32-TEST", "A8:B6:00:9C:FF:FF", "SENSOR-TEST-MOIST-KEY9-ZZZZZ");

        await coreDbContext.SaveChangesAsync();
    }
}