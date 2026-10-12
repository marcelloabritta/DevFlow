using DevFlow.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DevFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ProjectService>();
        return services;
    }
}
