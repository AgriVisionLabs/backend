using Agrivision.Backend.Infrastructure;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Agrivision.Backend.Api;

public static class DependencyInjections
{
    public static IServiceCollection AddDependencies(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.AddOpenApi();
        
        services.AddInfrastructureServices(config);

        return services;
    }
}