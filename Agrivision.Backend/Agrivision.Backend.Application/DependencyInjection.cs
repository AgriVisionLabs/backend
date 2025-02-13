using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Agrivision.Backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayerServices (this IServiceCollection services)
    {
        services.AddFluentValidationConfig();
        
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)
            .AddFluentValidationAutoValidation();
        
        return services;
    }
}