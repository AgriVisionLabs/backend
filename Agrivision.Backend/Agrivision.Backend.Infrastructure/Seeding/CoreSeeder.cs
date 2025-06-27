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
                SoilType = SoilType.Clay,
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

        try
        {
            var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
            if (adminUser == null)
                throw new Exception("Admin user not found.");

            if (await coreDbContext.Crops.AnyAsync())
            {
                logger.LogInformation("Crops already seeded.");
                return;
            }

            var now = DateTime.UtcNow;

            var crops = new List<Crop>
            {
                new Crop { Name = "Apple", CropType = CropType.Apple, Description = "Apples require a period of cold dormancy and are best suited for temperate climates.", GrowthDurationDays = 180, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 1, 2, 11, 12 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Bell Pepper", CropType = CropType.BellPepper, Description = "Bell peppers thrive in warm conditions and well-drained soils.", GrowthDurationDays = 90, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 3, 4, 5 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Blueberry", CropType = CropType.Blueberry, Description = "Blueberries prefer acidic, well-drained soil and moderate climates.", GrowthDurationDays = 120, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 6 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Cherry", CropType = CropType.Cherry, Description = "Cherries require a dormant season and grow well in temperate zones.", GrowthDurationDays = 90, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 11, 12, 1, 2, 3 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Corn", CropType = CropType.Corn, Description = "Corn is a warm-season cereal crop requiring full sun and fertile soils.", GrowthDurationDays = 120, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 3, 4, 5 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Grape", CropType = CropType.Grape, Description = "Grapes thrive in full sun and well-drained soils with moderate fertility.", GrowthDurationDays = 150, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 2, 3 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Orange", CropType = CropType.Orange, Description = "Oranges grow best in subtropical climates with well-drained soils.", GrowthDurationDays = 240, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 3, 4 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Peach", CropType = CropType.Peach, Description = "Peaches require chilling periods and well-drained, fertile soils.", GrowthDurationDays = 120, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 1, 2 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Potato", CropType = CropType.Potato, Description = "Potatoes prefer cool weather and loose, well-drained soils.", GrowthDurationDays = 90, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 10, 11, 12 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Raspberry", CropType = CropType.Raspberry, Description = "Raspberries grow in cool climates and prefer well-drained loamy soils.", GrowthDurationDays = 120, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 1, 2 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Soybean", CropType = CropType.Soybean, Description = "Soybeans are leguminous crops suited to warm climates and fertile soils.", GrowthDurationDays = 100, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 4, 5 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Squash", CropType = CropType.Squash, Description = "Squash grows best in warm weather and well-drained soils.", GrowthDurationDays = 70, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 3, 4, 5, 6 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Strawberry", CropType = CropType.Strawberry, Description = "Strawberries require cool temperatures and well-aerated soil.", GrowthDurationDays = 90, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 10, 11 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Tomato", CropType = CropType.Tomato, Description = "Tomatoes thrive in warm climates and fertile, well-drained soils.", GrowthDurationDays = 90, SupportsDiseaseDetection = true, PlantingMonths = new List<int> { 2, 3, 4, 5, 9, 10 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },

                // Crops without disease detection
                new Crop { Name = "Carrot", CropType = CropType.Carrot, Description = "Carrots grow well in cool conditions and deep, loose soil.", GrowthDurationDays = 75, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 10, 11 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Cucumber", CropType = CropType.Cucumber, Description = "Cucumbers prefer warm climates and moist, well-drained soil.", GrowthDurationDays = 60, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 3, 4, 5, 6 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Garlic", CropType = CropType.Garlic, Description = "Garlic grows best in loose, well-drained soil with cool temperatures.", GrowthDurationDays = 150, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 10, 11 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Lettuce", CropType = CropType.Lettuce, Description = "Lettuce thrives in cool seasons and fertile, well-drained soils.", GrowthDurationDays = 60, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 11, 12, 1 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Onion", CropType = CropType.Onion, Description = "Onions are cool-season crops preferring fertile, well-drained soil.", GrowthDurationDays = 150, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 10, 11 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Spinach", CropType = CropType.Spinach, Description = "Spinach prefers cool weather and moist, nutrient-rich soils.", GrowthDurationDays = 45, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 11, 12, 1 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Sweet Potato", CropType = CropType.SweetPotato, Description = "Sweet potatoes grow best in warm climates and sandy soils.", GrowthDurationDays = 120, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 3, 4 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Wheat", CropType = CropType.Wheat, Description = "Wheat grows in cooler weather and prefers fertile loamy soil.", GrowthDurationDays = 150, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 11, 12 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Rice", CropType = CropType.Rice, Description = "Rice requires flooded fields or high-moisture clay soils.", GrowthDurationDays = 150, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 5, 6 }, SoilType = SoilType.Clay, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Banana", CropType = CropType.Banana, Description = "Bananas grow year-round in tropical climates with moist, rich soil.", GrowthDurationDays = 270, SupportsDiseaseDetection = false, PlantingMonths = Enumerable.Range(1, 12).ToList(), SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Mango", CropType = CropType.Mango, Description = "Mangoes prefer tropical to subtropical climates and well-drained soil.", GrowthDurationDays = 150, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 3, 4 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Cabbage", CropType = CropType.Cabbage, Description = "Cabbage is a cool-season crop growing in fertile, moist soils.", GrowthDurationDays = 90, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 11, 12, 1 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Broccoli", CropType = CropType.Broccoli, Description = "Broccoli grows in cool climates and nutrient-rich, moist soils.", GrowthDurationDays = 90, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 11, 12, 1 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Beans", CropType = CropType.Beans, Description = "Beans are protein-rich legumes that grow in well-drained fertile soils.", GrowthDurationDays = 80, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 3, 4, 5 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Sesame", CropType = CropType.Sesame, Description = "Sesame is drought-tolerant and prefers hot climates with sandy soil.", GrowthDurationDays = 120, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 5, 6 }, SoilType = SoilType.Sandy, CreatedById = adminUser.Id, CreatedOn = now },
                new Crop { Name = "Peas", CropType = CropType.Peas, Description = "Peas are cool-season legumes that grow well in fertile, loamy soils.", GrowthDurationDays = 75, SupportsDiseaseDetection = false, PlantingMonths = new List<int> { 10, 11, 12 }, SoilType = SoilType.Loamy, CreatedById = adminUser.Id, CreatedOn = now }
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
    
    public static async Task SeedCropDiseasesAsync(IServiceProvider serviceProvider)
    {
        var coreDbContext = serviceProvider.GetRequiredService<CoreDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var options = serviceProvider.GetRequiredService<IOptions<AdminSettings>>();
        var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DiseaseSeeder");

        var adminSettings = options.Value;

        try
        {
            var adminUser = await userManager.FindByEmailAsync(adminSettings.Email);
            if (adminUser == null)
                throw new Exception("Admin user not found.");

            if (await coreDbContext.Diseases.AnyAsync())
            {
                logger.LogInformation("Diseases already seeded.");
                return;
            }

            var crops = await coreDbContext.Crops.ToListAsync();
            var cropDict = crops.ToDictionary(c => c.CropType.ToString(), c => c);

            var diseaseData = new List<(string CropType, string DiseaseName, List<string> Treatments)>
            {
                ("Apple", "Apple Scab", new List<string> { "Myclobutanil", "Captan" }),
                ("Apple", "Black Rot", new List<string> { "Copper-based fungicides", "Fluopyram" }),
                ("Apple", "Cedar Apple Rust", new List<string> { "Sulfur", "Myclobutanil" }),

                ("Cherry", "Powdery Mildew", new List<string> { "Sulfur", "Potassium bicarbonate" }),

                ("Corn", "Cercospora Leaf Spot", new List<string> { "Azoxystrobin", "Propiconazole" }),
                ("Corn", "Common Rust", new List<string> { "Trifloxystrobin", "Pyraclostrobin" }),
                ("Corn", "Northern Leaf Blight", new List<string> { "Azoxystrobin", "Propiconazole" }),

                ("Grape", "Black Rot", new List<string> { "Myclobutanil", "Fluopyram" }),
                ("Grape", "Esca (Black Measles)", new List<string> { "Trichoderma spp.", "Thiophanate-methyl" }), // Replaced sodium arsenite
                ("Grape", "Leaf Blight", new List<string> { "Copper-based fungicides", "Azoxystrobin" }),

                ("Orange", "Huanglongbing (Citrus Greening)", new List<string> { "Imidacloprid", "Thiamethoxam" }),

                ("Peach", "Bacterial Spot", new List<string> { "Copper hydroxide", "Kasugamycin" }), // Oxytetracycline minimized due to AMR concerns

                ("Pepper", "Bacterial Spot", new List<string> { "Copper hydroxide", "Zinc oxide" }),

                ("Potato", "Early Blight", new List<string> { "Chlorothalonil", "Mancozeb" }),
                ("Potato", "Late Blight", new List<string> { "Cymoxanil", "Mandipropamid" }), // Replacing Metalaxyl

                ("Squash", "Powdery Mildew", new List<string> { "Sulfur", "Potassium bicarbonate" }),

                ("Strawberry", "Leaf Scorch", new List<string> { "Captan", "Cyprodinil" }), // Safer alternative to Thiram

                ("Tomato", "Bacterial Spot", new List<string> { "Copper hydroxide", "Kasugamycin" }), // Replacing Streptomycin
                ("Tomato", "Early Blight", new List<string> { "Chlorothalonil", "Mancozeb" }),
                ("Tomato", "Late Blight", new List<string> { "Mandipropamid", "Cymoxanil" }),
                ("Tomato", "Leaf Mold", new List<string> { "Chlorothalonil", "Tebuconazole" }),
                ("Tomato", "Septoria Leaf Spot", new List<string> { "Chlorothalonil", "Azoxystrobin" }),
                ("Tomato", "Spider Mites", new List<string> { "Abamectin", "Spiromesifen" }),
                ("Tomato", "Target Spot", new List<string> { "Azoxystrobin", "Chlorothalonil" }),
                ("Tomato", "Tomato Yellow Leaf Curl Virus", new List<string> { "Imidacloprid", "Acetamiprid" }),
                ("Tomato", "Tomato Mosaic Virus", new List<string> { "Thermal therapy", "Cross-protection" })
            };

            var diseases = new List<CropDisease>();

            foreach (var (cropType, diseaseName, treatments) in diseaseData)
            {
                if (!cropDict.TryGetValue(cropType, out var crop))
                {
                    logger.LogWarning($"Crop '{cropType}' not found. Skipping disease '{diseaseName}'.");
                    continue;
                }

                diseases.Add(new CropDisease
                {
                    Name = diseaseName,
                    Treatments = treatments,
                    CropId = crop.Id,
                    CreatedById = adminUser.Id,
                    CreatedOn = DateTime.UtcNow
                });
            }

            await coreDbContext.Diseases.AddRangeAsync(diseases);
            await coreDbContext.SaveChangesAsync();

            logger.LogInformation("Seeded {Count} diseases into the database.", diseases.Count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to seed diseases.");
        }
    }
        
}
