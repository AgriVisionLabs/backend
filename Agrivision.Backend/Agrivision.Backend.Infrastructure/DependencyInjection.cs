using System.Text;
using Agrivision.Backend.Infrastructure.Auth;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Persistence.Identity.Entities;
using Agrivision.Backend.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Agrivision.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureLayerServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddApplicationUserDbContext(config)
            .AddAuthServices(config);
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

    private static IServiceCollection AddAuthServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtProvider, JwtProvider>();

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
}