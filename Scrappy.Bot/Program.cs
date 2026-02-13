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

    static IServiceProvider CreateProvider()
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
                var connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            })
            .AddBotServices()
            .AddBotHandlers()
            .AddRepositories();

        return services.BuildServiceProvider();
    }

    public static async Task Main()
    {
        DotNetEnv.Env.TraversePath().Load();
        _serviceProvider = CreateProvider();

        // Load database
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<BotDbContext>();
        await db.Database.MigrateAsync();

        // Start bot
        var bot = _serviceProvider.GetRequiredService<DiscordBotService>();
        await bot.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}