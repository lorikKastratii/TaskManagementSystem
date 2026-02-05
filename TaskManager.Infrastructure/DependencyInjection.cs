using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Interfaces;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Repositories;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<TaskDbContext>(options =>
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to InMemory if no connection string
                options.UseInMemoryDatabase("TaskManagerDb");
            }
            else
            {
                options.UseSqlServer(connectionString);
            }
        });

        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}
