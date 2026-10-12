using DevFlow.Application.Interfaces;
using DevFlow.Infrastructure.Persistence;
using DevFlow.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DevFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<DevFlowDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        return services;
    }
}
