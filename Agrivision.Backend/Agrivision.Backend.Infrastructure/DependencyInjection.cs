using System.Text;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Email;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Infrastructure.Auth;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Repositories;
using Agrivision.Backend.Infrastructure.Services.Email;
using Agrivision.Backend.Infrastructure.Settings;
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
        
        services.MapEmailSettings(config);

        services.AddEmailSender();
        
        services.MapAppSettings(config);
        
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

}