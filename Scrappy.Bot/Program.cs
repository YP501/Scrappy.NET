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
        // TODO: maybe set these up with global config
        var clientConfig = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent |
                             GatewayIntents.GuildMembers,
            MessageCacheSize = 500
        };
        var interactionConfig = new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Info,
        };
        var connectionString = DotNetEnv.Env.GetString("DB_CONNECTION_STRING");
        var services = new ServiceCollection()
            .AddSingleton(clientConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(interactionConfig)
            .AddDbContext<ScrappyDbContext>(options =>
            {
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            })
            .AddBotServices()
            .AddBotHandlers();

        return services.BuildServiceProvider();
    }

    public static async Task Main()
    {
        DotNetEnv.Env.TraversePath().Load();
        _serviceProvider = CreateProvider();

        // Load database
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ScrappyDbContext>();
        await db.Database.MigrateAsync();
        
        // Start bot
        var bot = _serviceProvider.GetRequiredService<DiscordBotService>();
        await bot.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}