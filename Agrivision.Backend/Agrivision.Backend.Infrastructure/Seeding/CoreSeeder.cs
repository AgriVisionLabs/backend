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
                CreatedById = adminUser.Id,
                FieldsNo = 2
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
    
    public static async Task SeedCropsAsync(IServiceProvider serviceProvider)
    {
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CropSeeder");
        
        
        var adminSettings = options.Value;

        if (await coreDbContext.Crops.AnyAsync())
        {
            logger.LogInformation("Crops already seeded.");
            return;
        }
            
        try
        {
            var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
            if (adminUser == null)
                throw new Exception("Couldn't find admin user.");

            if (await coreDbContext.Crops.AnyAsync())
            {
                logger.LogInformation("Crops already seeded.");
                return;
            }
            var now = DateTime.UtcNow;
            var crops = new List<Crop>
            {
                new Crop
                {
                    Name = "Apple",
                    CropType = CropType.Apple,
                    Description = "Apples are cultivated in Egypt's cooler regions, requiring specific chilling hours for optimal fruiting.",
                    GrowthDurationDays = 180,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 1, 2, 11, 12 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Bell Pepper",
                    CropType = CropType.BellPepper,
                    Description = "Bell peppers thrive in warm climates and are commonly grown in Egypt during the warmer months.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 3, 4, 5 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Blueberry",
                    CropType = CropType.Blueberry,
                    Description = "Blueberries have been successfully cultivated in Egypt, with harvests typically occurring from January to May.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 6 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Cherry",
                    CropType = CropType.Cherry,
                    Description = "Cherries require a dormant season and are best planted between November and March in Egypt.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 11, 12, 1, 2, 3 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Corn",
                    CropType = CropType.Corn,
                    Description = "Corn is a staple crop in Egypt, typically planted in the spring and harvested in the summer.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 3, 4, 5 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Grape",
                    CropType = CropType.Grape,
                    Description = "Grapes are widely cultivated in Egypt, with planting best done in late winter to early spring.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 2, 3 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Orange",
                    CropType = CropType.Orange,
                    Description = "Oranges are a major citrus crop in Egypt, with planting typically in the spring.",
                    GrowthDurationDays = 240,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 3, 4 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Peach",
                    CropType = CropType.Peach,
                    Description = "Peaches require chilling hours and are best planted in late winter in Egypt.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 1, 2 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Potato",
                    CropType = CropType.Potato,
                    Description = "Potatoes are commonly grown in Egypt during the cooler months.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 10, 11, 12 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Raspberry",
                    CropType = CropType.Raspberry,
                    Description = "Raspberries are less common in Egypt but can be grown in cooler regions.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 1, 2 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Soybean",
                    CropType = CropType.Soybean,
                    Description = "Soybeans are grown in Egypt's Nile Delta region, typically planted in the spring.",
                    GrowthDurationDays = 100,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 4, 5 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Squash",
                    CropType = CropType.Squash,
                    Description = "Squash is a warm-season crop in Egypt, planted in the spring and summer.",
                    GrowthDurationDays = 70,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 3, 4, 5, 6 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Strawberry",
                    CropType = CropType.Strawberry,
                    Description = "Strawberries are grown in Egypt during the cooler months, with planting in the fall.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 10, 11 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Tomato",
                    CropType = CropType.Tomato,
                    Description = "Tomatoes are a major crop in Egypt, grown year-round in various regions.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = true,
                    PlantingMonths = new List<int> { 2, 3, 4, 5, 9, 10 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Carrot",
                    CropType = CropType.Carrot,
                    Description = "Carrots are grown in Egypt during the cooler months, with planting in the fall.",
                    GrowthDurationDays = 75,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 10, 11 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Cucumber",
                    CropType = CropType.Cucumber,
                    Description = "Cucumbers are a warm-season crop in Egypt, planted in the spring and summer.",
                    GrowthDurationDays = 60,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 3, 4, 5, 6 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Garlic",
                    CropType = CropType.Garlic,
                    Description = "Garlic is planted in Egypt during the cooler months, typically in the fall.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 10, 11 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Lettuce",
                    CropType = CropType.Lettuce,
                    Description = "Lettuce is a cool-season crop in Egypt, grown during the winter months.",
                    GrowthDurationDays = 60,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 11, 12, 1 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Onion",
                    CropType = CropType.Onion,
                    Description = "Onions are planted in Egypt during the cooler months, with planting in the fall.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 10, 11 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Spinach",
                    CropType = CropType.Spinach,
                    Description = "Spinach is a cool-season crop in Egypt, grown during the winter months.",
                    GrowthDurationDays = 45,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 11, 12, 1 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Sweet Potato",
                    CropType = CropType.SweetPotato,
                    Description = "Sweet potatoes are a warm-season crop in Egypt, planted in the spring.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 3, 4 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Wheat",
                    CropType = CropType.Wheat,
                    Description = "Wheat is a major cereal crop in Egypt, planted in the winter.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 11, 12 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Rice",
                    CropType = CropType.Rice,
                    Description = "Rice is grown in Egypt's Nile Delta region, planted in the summer.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 5, 6 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Banana",
                    CropType = CropType.Banana,
                    Description = "Bananas are grown in Egypt's warmer regions, planted year-round.",
                    GrowthDurationDays = 270,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Mango",
                    CropType = CropType.Mango,
                    Description = "Mangoes are a major fruit crop in Egypt, planted in the spring.",
                    GrowthDurationDays = 150,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 3, 4 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Cabbage",
                    CropType = CropType.Cabbage,
                    Description = "Cabbage is a cool-season crop in Egypt, grown during the winter months.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 11, 12, 1 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Broccoli",
                    CropType = CropType.Broccoli,
                    Description = "Broccoli is a cool-season crop in Egypt, grown during the winter months.",
                    GrowthDurationDays = 90,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 11, 12, 1 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Beans",
                    CropType = CropType.Beans,
                    Description = "Beans are widely grown in Egypt during spring and early summer, valued for their high protein content.",
                    GrowthDurationDays = 80,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 3, 4, 5 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Sesame",
                    CropType = CropType.Sesame,
                    Description = "Sesame is a drought-tolerant crop grown in Egypt's hot summer climate.",
                    GrowthDurationDays = 120,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 5, 6 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                },
                new Crop
                {
                    Name = "Peas",
                    CropType = CropType.Peas,
                    Description = "Peas are cool-season legumes grown in Egypt during winter and early spring.",
                    GrowthDurationDays = 75,
                    SupportsDiseaseDetection = false,
                    PlantingMonths = new List<int> { 10, 11, 12 },
                    CreatedById = adminUser.Id,
                    CreatedOn = now
                }
            };

            await coreDbContext.Crops.AddRangeAsync(crops);
            await coreDbContext.SaveChangesAsync();

            logger.LogInformation("Seeded {Count} crops into the database.", crops.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed crops.");
        }
    }
        
}
