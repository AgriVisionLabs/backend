using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Services.Auth;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Agrivision.Backend.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        services.AddFluentValidationConfig();
        services.AddExceptionHandler();
        services.AddAuthService();
        
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

    private static IServiceCollection AddAuthService(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }

}