using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Hosting;

public class DiscordBotService : IHostedService
{
    private readonly DiscordSocketClient _client;

    public DiscordBotService(
        DiscordSocketClient client,
        LoggingService _,
        ReadyHandler __,
        MessageUpdatesHandler ___,
        MessageRecievedHandler ____)
    {
        _client = client;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        string token = DotNetEnv.Env.GetString("DISCORD_TOKEN_DEV");
        Console.WriteLine(token);
        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await _client.StopAsync();
    }
}
