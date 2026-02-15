using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scrappy.Data;

// "Script" that gets called by dotnet ef. For the developer only.
public class BotDbMigrationsGenerator : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.TraversePath().Load();
        var connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
        var dbVersion =  DotNetEnv.Env.GetString("DB_VERSION");

        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder.UseMySql(connectionString, new MariaDbServerVersion(dbVersion), options =>
        {
            options.MigrationsAssembly("Scrappy.Data");
        });

        return new BotDbContext(optionsBuilder.Options);
    }
}