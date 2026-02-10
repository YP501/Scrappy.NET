using Scrappy.Bot.Extensions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Scrappy.Bot.Services;

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
        var services = new ServiceCollection()
            .AddSingleton(clientConfig)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(interactionConfig)
            .AddBotServices()
            .AddBotHandlers();

        return services.BuildServiceProvider();
    }

    public static async Task Main()
    {
        _serviceProvider = CreateProvider();

        var bot = _serviceProvider.GetRequiredService<DiscordBotService>();
        await bot.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}