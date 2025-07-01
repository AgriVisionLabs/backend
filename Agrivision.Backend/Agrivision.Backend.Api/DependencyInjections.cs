using Agrivision.Backend.Api.Services.Hubs;
using Agrivision.Backend.Application;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Infrastructure;
using Agrivision.Backend.Infrastructure.Services.Hubs;

namespace Agrivision.Backend.Api;

public static class DependencyInjections
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        
        services.AddOpenApi();

        services.AddInfrastructureLayerServices(config);

        services.AddApplicationLayerServices();

        services.AddCors();

        services.AddSignalR();

        services.AddHubServices();

        services.AddSensorReadingBroadcaster();

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin", builder =>
            {
                builder.WithOrigins("http://localhost:5173", "https://www.agrivisionlabs.tech", "https://agrivisionlabs.tech", "https://agrivision-web.vercel.app") 
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        return services;
    }
    
    private static IServiceCollection AddHubServices(this IServiceCollection services)
    {
        services.AddScoped<ISensorHubNotifier, SensorHubNotifier>();

        return services;
    }
    
    private static IServiceCollection AddSensorReadingBroadcaster(this IServiceCollection services)
    {
        services.AddScoped<ISensorReadingBroadcaster, SensorReadingBroadcaster>();

        return services;
    }
    
    

}