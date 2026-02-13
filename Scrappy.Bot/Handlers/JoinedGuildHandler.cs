using Discord.WebSocket;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;

public class JoinedGuildHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly GuildConfigService _configService;

    public JoinedGuildHandler(DiscordSocketClient client, GuildConfigService configService)
    {
        _client = client;
        _configService = configService;
    }

    public Task InitializeAsync()
    {
        _client.JoinedGuild += OnJoinedGuildAsync;
        return Task.CompletedTask;
    }

    private async Task OnJoinedGuildAsync(SocketGuild guild)
    {
        // Initialize guild configuration
        await _configService.GetOrAddConfigAsync(guild.Id);
    }
}