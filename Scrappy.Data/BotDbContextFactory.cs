using DotNetEnv;
using DotNetEnv.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Scrappy.Data;

// This class is exclusively used by the 'dotnet ef' CLI tool.
public class BotDbContextFactory : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddDotNetEnv(".env", LoadOptions.TraversePath())
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString)) throw new ArgumentException("DefaultConnection not provided in environment variables.");

        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new BotDbContext(optionsBuilder.Options);
    }
}