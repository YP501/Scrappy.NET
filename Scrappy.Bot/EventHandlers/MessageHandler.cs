using Discord;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;

public class MessageHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly LevelService _levelService;

    public MessageHandler(DiscordSocketClient client, LevelService levelService)
    {
        _client = client;
        _levelService = levelService;
    }

    public Task InitializeAsync()
    {
        _client.MessageReceived += OnMessageReceived;
        _client.MessageUpdated += OnMessageUpdatedAsync;

        return Task.CompletedTask;
    }

    private Task OnMessageReceived(SocketMessage msg)
    {
        // Don't let thread hang on this async method AKA fire and forget
        if (msg is SocketUserMessage userMessage) _ = _levelService.ProcessMessageXpAsync(userMessage);

        return Task.CompletedTask;
    }

    private async Task OnMessageUpdatedAsync(
        Cacheable<IMessage, ulong> msgBefore,
        SocketMessage msgAfter,
        ISocketMessageChannel channel)
    {
        if (msgAfter.Author.IsBot) return;

        var message = await msgBefore.GetOrDownloadAsync();
        Console.WriteLine($"Message updated in {channel.Name}: {message?.CleanContent} -> {msgAfter.CleanContent}");
    }
}