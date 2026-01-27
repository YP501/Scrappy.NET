using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    private static IServiceProvider? _serviceProvider;
    static IServiceProvider CreateProvider()
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds |
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent |
                             GatewayIntents.GuildMembers,
            MessageCacheSize = 500
        };
        var services = new ServiceCollection()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton<LoggingService>()
            .AddSingleton<ReadyHandler>()
            .AddSingleton<MessageUpdatesHandler>()
            .AddSingleton<MessageRecievedHandler>();

        return services.BuildServiceProvider();
    }

    public static async Task Main()
    {
        _serviceProvider = CreateProvider();

        // Load credentials
        DotNetEnv.Env.Load();
        var token = DotNetEnv.Env.GetString("DISCORD_TOKEN_DEV");

        // Activate services that need to start immediately
        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        _serviceProvider.GetRequiredService<LoggingService>();
        _serviceProvider.GetRequiredService<ReadyHandler>();
        _serviceProvider.GetRequiredService<MessageUpdatesHandler>();
        _serviceProvider.GetRequiredService<MessageRecievedHandler>();

        // Login and start Discord bot
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        // Keep running
        await Task.Delay(-1);
    }
}