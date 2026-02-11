using Discord;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;

namespace Scrappy.Bot.Services;

public class DiscordBotService
{
    private readonly DiscordSocketClient _client;
    private readonly IEnumerable<IEventHandler> _handlers;

    public DiscordBotService(
        DiscordSocketClient client,
        IEnumerable<IEventHandler> handlers
        )
    {
        _client = client;
        _handlers = handlers;
    }

    public async Task StartAsync()
    {
        // Start handlers
        foreach (var handler in _handlers)
        {
            await handler.InitializeAsync();
        }
        
        // Login and start bot
        await _client.LoginAsync(TokenType.Bot, DotNetEnv.Env.GetString("DISCORD_TOKEN"));
        await _client.StartAsync();
    }
}