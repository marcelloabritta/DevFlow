using DevFlow.Application.Services;

using Microsoft.Extensions.DependencyInjection;

namespace DevFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ProjectService>();
        services.AddScoped<TaskItemService>();
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();

        return services;
    }
}
