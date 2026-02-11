using Discord;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;

namespace Scrappy.Bot.Handlers;

public class MessageUpdatesHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    public MessageUpdatesHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public Task InitializeAsync()
    {
        _client.MessageUpdated += OnMessageUpdatedAsync;
        return Task.CompletedTask;
    }

    private async Task OnMessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        if (after.Author.IsBot) return;
        
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"Message updated in {channel.Name}: {message?.CleanContent} -> {after.CleanContent}");
    }
}