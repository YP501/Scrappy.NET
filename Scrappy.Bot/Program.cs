using Scrappy.Bot.Extensions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Services;
using Scrappy.Data;

namespace Scrappy.Bot;

public static class Program
{
    private static IServiceProvider? _serviceProvider;

    private static IServiceProvider CreateProvider()
    {
        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildBans |
                             GatewayIntents.GuildMembers |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent,
            MessageCacheSize = 500,
            AuditLogCacheSize = 0,
        };
        var interactionConfig = new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Info,
        };
        var services = new ServiceCollection()
            .AddSingleton(clientConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(interactionConfig)
            .AddDbContext<BotDbContext>(options =>
            {
                string connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
                string dbVersion = DotNetEnv.Env.GetString("DB_VERSION");
                options.UseMySql(connectionString, new MariaDbServerVersion(dbVersion), o =>
                {
                    o.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
            })
            .AddBotServices()
            .AddBotHandlers()
            .AddRepositories();

        return services.BuildServiceProvider();
    }

    private static async Task ApplyDatabaseMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        await db.Database.MigrateAsync();
    }

    private static void InitializeAndCheckEnvs()
    {
        DotNetEnv.Env.TraversePath().Load();
        string[] requiredEnvs =
        {
            "DISCORD_TOKEN",
            "DB_CONNECTION_STRING",
            "DB_VERSION",
        };

        var missingEnvs = requiredEnvs.Where(env => string.IsNullOrEmpty(DotNetEnv.Env.GetString(env))).ToList();

        if (missingEnvs.Any())
        {
            Console.WriteLine("Critical Error: Unknown configuration!");
            missingEnvs.ForEach(env =>
            {
                Console.WriteLine($"     -> Missing env entry `{env}`");
            });
            Console.WriteLine("Make sure your .env file is complete and try again");
            return;
        }
    }

    public static async Task Main()
    {
        InitializeAndCheckEnvs();
        _serviceProvider = CreateProvider();

        // Update database tables with migrations in a scope to force proper disposal of DbContext
        await ApplyDatabaseMigrationsAsync(_serviceProvider);

        // Start bot
        var bot = _serviceProvider.GetRequiredService<DiscordBotService>();
        await bot.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}