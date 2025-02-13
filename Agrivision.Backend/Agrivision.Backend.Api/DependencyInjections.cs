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

        return services;
    }
}