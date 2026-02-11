using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Scrappy.Data;

// "Script" that gets called by dotnet ef. For the developer only.
public class ScrappyDbMigrationsGenerator : IDesignTimeDbContextFactory<ScrappyDbContext>
{
    public ScrappyDbContext CreateDbContext(string[] args)
    {
        DotNetEnv.Env.TraversePath().Load();
        var connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");

        var optionsBuilder = new DbContextOptionsBuilder<ScrappyDbContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), b =>
            b.MigrationsAssembly("Scrappy.Data"));

        return new ScrappyDbContext(optionsBuilder.Options);
    }
}