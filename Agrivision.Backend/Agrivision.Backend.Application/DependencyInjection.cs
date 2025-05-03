using System.Reflection;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Agrivision.Backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddFluentValidationConfig();
        services.AddExceptionHandler();
        services.AddMediatRConfiguration();
        services.AddApplicationLayerSettings();
        services.AddMapsterConfigurations();
        
        return services;
    }

    private static IServiceCollection AddFluentValidationConfig(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)
            .AddFluentValidationAutoValidation();

        return services;
    }

    private static IServiceCollection AddExceptionHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>(); // to register the GlobalExceptionHandler as the default exception handler
        services.AddProblemDetails(); // to be able to use the problem details class ig

        return services;
    }
    

    private static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly)); // registers all the commands and queries (must implement IRequest) and all the handlers (must implement IRequestHandler)

        return services;
    }

    private static IServiceCollection AddApplicationLayerSettings(this IServiceCollection services)
    {
        services.AddOptions<RefreshTokenSettings>()
            .BindConfiguration(RefreshTokenSettings.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }

    private static IServiceCollection AddMapsterConfigurations(this IServiceCollection services)
    {
        var mappingConfig = TypeAdapterConfig.GlobalSettings;
        mappingConfig.Scan(typeof(ApplicationAssemblyMarker).Assembly);

        return services;
    }
}