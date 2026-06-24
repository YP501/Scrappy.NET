using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Repositories;

namespace Scrappy.Data.Extensions;

public static class DataServiceExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register repositories
        services.AddScoped<IInfractionRepository, InfractionRepository>();
        services.AddScoped<IGuildConfigRepository, GuildConfigRepository>();
        services.AddScoped<ILevelUserRepository, LevelUserRepository>();

        // Register DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("DefaultConnection not provided in environment variables.");

        services.AddDbContext<BotDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        return services;
    }
}