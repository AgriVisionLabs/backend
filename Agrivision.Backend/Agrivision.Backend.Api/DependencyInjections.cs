using Agrivision.Backend.Application;
using Agrivision.Backend.Infrastructure;

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

        return services;
    }

    private static IServiceCollection AddCors(this IServiceCollection services)
    {
        services.AddCors(options =>
            options.AddPolicy("AllowAny", builder =>
                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
            )
        );

        return services;
    }

}