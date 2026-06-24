using Discord.WebSocket;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;

public class GuildHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly GuildConfigService _configService;

    public GuildHandler(DiscordSocketClient client, GuildConfigService configService)
    {
        _client = client;
        _configService = configService;
    }

    public Task InitializeAsync()
    {
        _client.LeftGuild += OnLeftGuildAsync;
        _client.JoinedGuild += OnJoinedGuildAsync;

        return Task.CompletedTask;
    }

    private Task OnLeftGuildAsync(SocketGuild guild)
    {
        _configService.RemoveFromCache(guild.Id);
        return Task.CompletedTask;
    }

    private async Task OnJoinedGuildAsync(SocketGuild guild)
    {
        await _configService.GetOrAddConfigAsync(guild.Id);
    }
}