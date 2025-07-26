using System.Text;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Repositories.Core;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.DiseaseDetection;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Services.Files;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Application.Services.InvitationTokenGenerator;
using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Application.Services.Otp;
using Agrivision.Backend.Application.Services.Payment;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Infrastructure.Auth;
using Agrivision.Backend.Infrastructure.Background;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Repositories.Core;
using Agrivision.Backend.Infrastructure.Repositories.Identity;
using Agrivision.Backend.Infrastructure.Services.DiseaseDetection;
using Agrivision.Backend.Infrastructure.Services.Email;
using Agrivision.Backend.Infrastructure.Services.Files;
using Agrivision.Backend.Infrastructure.Services.Hubs;
using Agrivision.Backend.Infrastructure.Services.InvitationTokenGenerator;
using Agrivision.Backend.Infrastructure.Services.IoT;
using Agrivision.Backend.Infrastructure.Services.Otp;
using Agrivision.Backend.Infrastructure.Services.Payment;
using Agrivision.Backend.Infrastructure.Settings;
using Agrivision.Backend.Infrastructure.WebSockets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace Agrivision.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services,
        IConfiguration config)
    {
        services.AddApplicationUserDbContext(config);

        services.AddCoreDbContext(config);
        
        services.AddIdentityServices();

        services.AddAuthenticationServices(config);
        
        services.AddGoogleAuthService();
        
        services.MapEmailSettings(config);

        services.AddEmailSender();
        
        services.MapAppSettings(config);

        services.AddFarmRepository();

        services.AddFieldRepository();

        services.MapAdminSettings(config);

        services.AddFarmRoleRepository();

        services.AddFarmUserRoleRepository();

        services.AddFarmRoleClaimRepository();

        services.AddGlobalRoleRepository();

        services.AddInvitationTokenService();

        services.AddFarmInvitationRepository();

        services.AddOtpProvider();

        services.AddOtpVerificationRepository();

        services.AddInfrastructureLayerSettings();

        services.AddWebSocketConnectionHandler();

        services.AddWebSocketDeviceCommunicator();

        services.AddIrrigationUnitDeviceWebSocketHandler();

        services.AddSubscriptionPlanRepository();

        services.AddUserSubscriptionRepository();
        
        services.AddStripeService();

        services.AddIrrigationUnitRepository();

        services.AddIrrigationUnitDeviceRepository();

        services.AddIrrigationUnitDeviceHeartbeatService();

        services.AddSensorUnitDeviceWebSocketHandler();

        services.AddOtpServices();

        services.AddSensorUnitDeviceHeartbeatService();

        services.AddSensorUnitDeviceRepository();
        
        services.AddSensorUnitRepository();

        services.AddSensorReadingRepository();

        services.AddAutomationRuleRepository();
        
        services.AddAutomationRuleExecutionService();

        services.AddFarmConnectionTracker();

        services.AddTaskItemRepository();

        services.AddIrrigationEventRepository();

        services.AddInventoryItemRepository();

        services.AddInventoryItemTransactionRepository();

        services.AddPlantedCropRepository();

        services.AddCropRepository();

        services.AddDiseaseDetectionRepository();

        services.AddCropDiseaseRepository();

        services.MapDiseaseDetectionSettings(config);

        services.AddFileUploadService();

        services.AddVideoProcessingService();

        services.AddImageProcessingService();

        services.AddDiseaseDetectionService();

        services.MapStripeSettings(config);

        services.AddUserContext();

        services.AddConversationRepository();

        services.AddMessageRepository();

        services.AddConversationInviteLogRepository();

        services.AddConversationMemberRepository();

        services.AddClearedConversationRepository();
        
        services.AddConversationConnectionTracker();

        services.AddUserConnectionTracker();

        services.AddNotificationRepository();

        services.AddNotificationPreferenceRepository();
        
        services.AddReadNotificationRepository();
        
        services.AddClearedNotificationRepository();

        return services;
    }

    private static IServiceCollection AddApplicationUserDbContext(this IServiceCollection services,
        IConfiguration config)
    {
        var identityDbConnectionString = config.GetConnectionString("IdentityDbConnectionString") ??
                                         throw new InvalidOperationException(
                                             "Can't find IdentityDb connection string.");
        services.AddDbContext<ApplicationUserDbContext>(options => options.UseSqlServer(identityDbConnectionString));

        return services;
    }

    private static IServiceCollection AddCoreDbContext(this IServiceCollection services, IConfiguration config)
    {
        var coreDbConnectionString = config.GetConnectionString("CoreDbConnectionString") ??
                                     throw new InvalidOperationException("Can't find CoreDb connection string.");

        services.AddDbContext<CoreDbContext>(options => options.UseSqlServer(coreDbConnectionString));
        
        return services;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password Configuration
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                // Lockout Configuration
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                options.Lockout.MaxFailedAccessAttempts = 5;

                // Email Configuration
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationUserDbContext>()  // Store identity data in our DB
            .AddDefaultTokenProviders(); // Enables email confirmation, password reset, etc.

        // Register User Repository
        services.AddScoped<IUserRepository, UserRepository>();
        
        // Register Auth Repository
        services.AddScoped<IAuthRepository, AuthRepository>();
        
        return services;
    }
    
    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();

        services.AddOptions<JwtOptions>()
            .BindConfiguration(JwtOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var jwtSettings = config.GetSection(JwtOptions.SectionName).Get<JwtOptions>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.SaveToken = true;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings?.Key!)),
                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                };
                
                // the magic for SignalR
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/hub/sensors") || path.StartsWithSegments("/hubs/messages") ||
                                                                   path.StartsWithSegments("/hubs/conversations")))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    public static IHostBuilder AddSerilog(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, configuration) =>
        {
            var environment = context.HostingEnvironment.EnvironmentName;

            configuration.ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithProcessId();

            if (environment == "Development")
            {
                configuration
                    .MinimumLevel.Information()
                    .WriteTo.Console(outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} " +
                        "[Machine: {MachineName}] [ThreadId: {ThreadId}] [Process: {ProcessId}]{NewLine}{Exception}")
                    .WriteTo.File("../Agrivision/logs/dev-log-.log", rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} " +
                                        "[Machine: {MachineName}] [ThreadId: {ThreadId}] [Process: {ProcessId}]{NewLine}{Exception}");
            }
            else // Production
            {
                configuration
                    .MinimumLevel.Warning()
                    .WriteTo.File("../Agrivision/logs/log-.log", rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} " +
                                        "[Machine: {MachineName}] [ThreadId: {ThreadId}] [Process: {ProcessId}]{NewLine}{Exception}");
            }
        });

        return hostBuilder;
    }

    private static IServiceCollection MapEmailSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MailSettings>(configuration.GetSection(nameof(MailSettings)));

        return services;
    }

    private static IServiceCollection MapAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));

        return services;
    }

    private static IServiceCollection AddEmailSender(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }

    private static IServiceCollection AddFarmRepository(this IServiceCollection services)
    {
        services.AddScoped<IFarmRepository, FarmRepository>();

        return services;
    }

    private static IServiceCollection AddFieldRepository(this IServiceCollection services)
    {
        services.AddScoped<IFieldRepository, FieldRepository>();

        return services;
    }
    
    private static IServiceCollection MapAdminSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AdminSettings>(configuration.GetSection(nameof(AdminSettings)));

        return services;
    }
    
    private static IServiceCollection AddFarmRoleRepository(this IServiceCollection services)
    {
        services.AddScoped<IFarmRoleRepository, FarmRoleRepository>();

        return services;
    }
    
    private static IServiceCollection AddFarmUserRoleRepository(this IServiceCollection services)
    {
        services.AddScoped<IFarmUserRoleRepository, FarmUserRoleRepository>();

        return services;
    }

    private static IServiceCollection AddFarmRoleClaimRepository(this IServiceCollection services)
    {
        services.AddScoped<IFarmRoleClaimRepository, FarmRoleClaimRepository>();

        return services;
    }

    private static IServiceCollection AddGlobalRoleRepository(this IServiceCollection services)
    {
        services.AddScoped<IGlobalRoleRepository, GlobalRoleRepository>();

        return services;
    }
    
    private static IServiceCollection AddInvitationTokenService(this IServiceCollection services)
    {
        services.AddScoped<IInvitationTokenGenerator, InvitationTokenGenerator>();

        return services;
    }
    
    private static IServiceCollection AddFarmInvitationRepository(this IServiceCollection services)
    {
        services.AddScoped<IFarmInvitationRepository, FarmInvitationRepository>();

        return services;
    }
    
    private static IServiceCollection AddOtpProvider(this IServiceCollection services)
    {
        services.AddScoped<IOtpProvider, OtpProvider>();

        return services;
    }
    
    private static IServiceCollection AddInfrastructureLayerSettings(this IServiceCollection services)
    {
        services.AddOptions<AppSettings>()
            .BindConfiguration(AppSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<MailSettings>()
            .BindConfiguration(MailSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<AdminSettings>()
            .BindConfiguration(AdminSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<OtpSettings>()
            .BindConfiguration(OtpSettings.SectionName)
            .ValidateDataAnnotations()
            .Validate(otp => otp.Verification.MaxAttempts > 0 && otp.PasswordReset.MaxAttempts > 0 && otp.TwoFactor.MaxAttempts > 0, "OTP configuration must be valid.")
            .ValidateOnStart();
        
        services.AddOptions<DiseaseDetectionSettings>()
            .BindConfiguration(DiseaseDetectionSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<GoogleAuthSettings>()
            .BindConfiguration(GoogleAuthSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }

    private static IServiceCollection AddWebSocketConnectionHandler(this IServiceCollection services)
    {
        services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>();

        return services;
    }
    
    private static IServiceCollection AddWebSocketDeviceCommunicator(this IServiceCollection services)
    {
        services.AddScoped<IWebSocketDeviceCommunicator, WebSocketDeviceCommunicator>();

        return services;
    }

    private static IServiceCollection AddIrrigationUnitDeviceWebSocketHandler(this IServiceCollection services)
    {
        services.AddScoped<IrrigationUnitDeviceWebSocketHandler>();

        return services;
    }
    
    private static IServiceCollection AddSensorUnitDeviceWebSocketHandler(this IServiceCollection services)
    {
        services.AddScoped<SensorUnitDeviceWebSocketHandler>();

        return services;
    }
    
    private static IServiceCollection AddSubscriptionPlanRepository(this IServiceCollection services)
    {
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        return services;
    }
    
    private static IServiceCollection AddUserSubscriptionRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        return services;
    }
    
    private static IServiceCollection AddStripeService(this IServiceCollection services)
    {
        services.AddScoped<IStripeService, StripeService>();
        return services;
    }

    private static IServiceCollection AddIrrigationUnitRepository(this IServiceCollection services)
    {
        services.AddScoped<IIrrigationUnitRepository, IrrigationUnitRepository>();

        return services;
    }
    
    private static IServiceCollection AddIrrigationUnitDeviceRepository(this IServiceCollection services)
    {
        services.AddScoped<IIrrigationUnitDeviceRepository, IrrigationUnitDeviceRepository>();

        return services;
    }
    
    private static IServiceCollection AddIrrigationUnitDeviceHeartbeatService(this IServiceCollection services)
    {
        services.AddSingleton<IrrigationUnitDeviceHeartbeatService>();
        services.AddHostedService(provider => provider.GetRequiredService<IrrigationUnitDeviceHeartbeatService>());

        return services;
    }
    
    private static IServiceCollection AddOtpVerificationRepository(this IServiceCollection services)
    {
        services.AddScoped<IOtpVerificationRepository, OtpVerificationRepository>();

        return services;
    }
    
    private static IServiceCollection AddOtpServices(this IServiceCollection services)
    {
        services.AddScoped<IOtpHashingService, OtpHashingService>();
        
        services.AddScoped<IOtpGenerator, OtpGenerator>();

        services.AddScoped<IOtpProvider, OtpProvider>();

        return services;
    }
    
    private static IServiceCollection AddSensorUnitDeviceHeartbeatService(this IServiceCollection services)
    {
        services.AddSingleton<SensorUnitDeviceHeartbeatService>();
        services.AddHostedService(provider => provider.GetRequiredService<SensorUnitDeviceHeartbeatService>());

        return services;
    }
    
    private static IServiceCollection AddSensorUnitDeviceRepository(this IServiceCollection services)
    {
        services.AddScoped<ISensorUnitDeviceRepository, SensorUnitDeviceRepository>();

        return services;
    }
    
    private static IServiceCollection AddSensorUnitRepository(this IServiceCollection services)
    {
        services.AddScoped<ISensorUnitRepository, SensorUnitRepository>();

        return services;
    }
    
    private static IServiceCollection AddSensorReadingRepository(this IServiceCollection services)
    {
        services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();

        return services;
    }
    
    private static IServiceCollection AddAutomationRuleRepository(this IServiceCollection services)
    {
        services.AddScoped<IAutomationRuleRepository, AutomationRuleRepository>();

        return services;
    }
    
    private static IServiceCollection AddAutomationRuleExecutionService(this IServiceCollection services)
    {
        services.AddSingleton<AutomationRuleExecutionService>();
        services.AddHostedService(provider => provider.GetRequiredService<AutomationRuleExecutionService>());

        return services;
    }
    
    private static IServiceCollection AddFarmConnectionTracker(this IServiceCollection services)
    {
        services.AddSingleton<IFarmConnectionTracker, FarmConnectionTracker>();

        return services;
    }

    private static IServiceCollection AddTaskItemRepository(this IServiceCollection services)
    {
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return services;
    }

    private static IServiceCollection AddIrrigationEventRepository(this IServiceCollection services)
    {
        services.AddScoped<IIrrigationEventRepository, IrrigationEventRepository>();

        return services;
    }

    private static IServiceCollection AddInventoryItemRepository(this IServiceCollection services)
    {
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();

        return services;
    }
    
    private static IServiceCollection AddInventoryItemTransactionRepository(this IServiceCollection services)
    {
        services.AddScoped<IInventoryItemTransactionRepository, InventoryItemTransactionRepository>();

        return services;
    }
    
    private static IServiceCollection AddPlantedCropRepository(this IServiceCollection services)
    {
        services.AddScoped<IPlantedCropRepository, PlantedCropRepository>();

        return services;
    }
    
    private static IServiceCollection AddCropRepository(this IServiceCollection services)
    {
        services.AddScoped<ICropRepository, CropRepository>();

        return services;
    }
    
    private static IServiceCollection AddDiseaseDetectionRepository(this IServiceCollection services)
    {
        services.AddScoped<IDiseaseDetectionRepository, DiseaseDetectionRepository>();

        return services;
    }
    
    private static IServiceCollection AddCropDiseaseRepository(this IServiceCollection services)
    {
        services.AddScoped<ICropDiseaseRepository, CropDiseaseRepository>();

        return services;
    }
    
    private static IServiceCollection MapDiseaseDetectionSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DiseaseDetectionSettings>(configuration.GetSection(nameof(DiseaseDetectionSettings)));

        return services;
    }
    
    private static IServiceCollection AddDiseaseDetectionService(this IServiceCollection services)
    {
        services.AddScoped<IDiseaseDetectionService, DiseaseDetectionService>();
        services.AddHttpClient<IDiseaseDetectionService, DiseaseDetectionService>();

        return services;
    }
    
    private static IServiceCollection AddFileUploadService(this IServiceCollection services)
    {
        services.AddScoped<IFileUploadService, FileUploadService>();

        return services;
    }
    
    private static IServiceCollection AddVideoProcessingService(this IServiceCollection services)
    {
        services.AddScoped<IVideoProcessingService, VideoProcessingService>();

        return services;
    }
    
    private static IServiceCollection AddImageProcessingService(this IServiceCollection services)
    {
        services.AddScoped<IImageProcessingService, ImageProcessingService>();

        return services;
    }
    
    private static IServiceCollection MapStripeSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StripeSettings>(configuration.GetSection(nameof(StripeSettings)));

        return services;
    }
    
    private static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
    
    private static IServiceCollection AddConversationRepository(this IServiceCollection services)
    {
        services.AddScoped<IConversationRepository, ConversationRepository>();

        return services;
    }
    
    private static IServiceCollection AddMessageRepository(this IServiceCollection services)
    {
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
    
    private static IServiceCollection AddClearedConversationRepository(this IServiceCollection services)
    {
        services.AddScoped<IClearedConversationRepository, ClearedConversationRepository>();

        return services;
    }
    
    private static IServiceCollection AddConversationInviteLogRepository(this IServiceCollection services)
    {
        services.AddScoped<IConversationInviteLogRepository, ConversationInviteLogRepository>();

        return services;
    }

    private static IServiceCollection AddConversationMemberRepository(this IServiceCollection services)
    {
        services.AddScoped<IConversationMemberRepository, ConversationMemberRepository>();

        return services;
    }
    
    private static IServiceCollection AddConversationConnectionTracker(this IServiceCollection services)
    {
        services.AddSingleton<IConversationConnectionTracker, ConversationConnectionTracker>();

        return services;
    }
    
    private static IServiceCollection AddUserConnectionTracker(this IServiceCollection services)
    {
        services.AddSingleton<IUserConnectionTracker, UserConnectionTracker>();

        return services;
    }

    private static IServiceCollection AddGoogleAuthService(this IServiceCollection services)
    {
        services.AddScoped<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
    
    private static IServiceCollection AddNotificationRepository(this IServiceCollection services)
    {
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
    
    private static IServiceCollection AddNotificationPreferenceRepository(this IServiceCollection services)
    {
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

        return services;
    }
    
    private static IServiceCollection AddReadNotificationRepository(this IServiceCollection services)
    {
        services.AddScoped<IReadNotificationRepository, ReadNotificationRepository>();

        return services;
    }
    
    private static IServiceCollection AddClearedNotificationRepository(this IServiceCollection services)
    {
        services.AddScoped<IClearedNotificationRepository, ClearedNotificationRepository>();

        return services;
    }
}