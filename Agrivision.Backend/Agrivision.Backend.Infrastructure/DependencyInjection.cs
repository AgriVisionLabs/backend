using System.Text;
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Repositories;
using Agrivision.Backend.Application.Services.Auth;
using Agrivision.Backend.Infrastructure.Auth;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Repositories;
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
        services.AddApplicationUserDbContext(config)
            .AddIdentityAuth(config)
            .AddIdentityConfigurations();
        return services;
    }

    private static IServiceCollection AddApplicationUserDbContext(this IServiceCollection services,
        IConfiguration config)
    {
        var identityDbConnectionString = config.GetConnectionString("IdentityDbConnectionString") ??
                                         throw new InvalidOperationException(
                                             "Can't find IdentityDb Connection String lil bro");
        services.AddDbContext<ApplicationUserDbContext>(options => options.UseSqlServer(identityDbConnectionString));

        return services;
    }

    private static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IJwtProvider, JwtProvider>();
        services.AddScoped<IUserRepository, IdentityRepository>();

        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationUserDbContext>();

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
                // Development logging (Verbose, Console + File)
                configuration
                    .MinimumLevel.Information()
                    .WriteTo.Console(outputTemplate:
                        "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} " +
                        "[Machine: {MachineName}] [ThreadId: {ThreadId}] [Process: {ProcessId}]{NewLine}{Exception}")
                    .WriteTo.File("../Logs/dev-log-.log", rollingInterval: RollingInterval.Day,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message} " +
                                        "[Machine: {MachineName}] [ThreadId: {ThreadId}] [Process: {ProcessId}]{NewLine}{Exception}");
            }
            else // Production
            {
                // Production logging (Optimized, JSON format, File only)
                configuration.MinimumLevel.Warning()
                    .WriteTo.File(
                        formatter: new Serilog.Formatting.Compact.CompactJsonFormatter(),
                        path: "../agrivision/log-.json",
                        rollingInterval: RollingInterval.Day
                    );
            }
        });

        return hostBuilder;
    }

    private static IServiceCollection AddIdentityConfigurations(this IServiceCollection services)
    {
        
        services.Configure<IdentityOptions>(options =>
        {
            // password configuration 
            options.Password.RequiredLength = 8;
            // email configuration
            options.SignIn.RequireConfirmedEmail = true;
            options.User.RequireUniqueEmail = true;
        });
        

        return services;
    }

}