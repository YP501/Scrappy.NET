using Discord.WebSocket;
using Discord;

public class MessageUpdatesHandler
{
    private readonly DiscordSocketClient _client;
    public MessageUpdatesHandler(DiscordSocketClient client)
    {
        _client = client;
        _client.MessageUpdated += OnMessageUpdatedAsync;
    }

    private async Task OnMessageUpdatedAsync(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
    {
        var message = await before.GetOrDownloadAsync();
        Console.WriteLine($"Message updated in {channel.Name}: {message?.Content} -> {after.Content}");
    }
}