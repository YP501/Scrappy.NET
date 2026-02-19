using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scrappy.Data;

// "Script" that gets called by dotnet ef. For the developer only.
public class BotDbMigrationsGenerator : IDesignTimeDbContextFactory<BotDbContext>
{
    public BotDbContext CreateDbContext(string[] args)
    {
        Env.TraversePath().Load();
        var connectionString = Env.GetString("DB_CONNECTION_STRING");
        var dbVersion = Env.GetString("DB_VERSION");

        var optionsBuilder = new DbContextOptionsBuilder<BotDbContext>();
        optionsBuilder.UseMySql(connectionString, new MariaDbServerVersion(dbVersion),
            options => { options.MigrationsAssembly("Scrappy.Data"); });

        return new BotDbContext(optionsBuilder.Options);
    }
}