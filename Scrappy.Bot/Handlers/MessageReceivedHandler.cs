using Discord;
using Discord.WebSocket;
using Scrappy.Bot.Interfaces;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Handlers;
public class MessageReceivedHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    private readonly LevelService _levelService;
    public MessageReceivedHandler(DiscordSocketClient client, LevelService levelService)
    {
        _client = client;
        _levelService = levelService;
    }

    public Task InitializeAsync()
    {
        _client.MessageReceived += OnMessageReceived;
        return Task.CompletedTask;
    }

    private Task OnMessageReceived(SocketMessage msg)
    {
        if (msg is SocketUserMessage userMessage)
        {
            _ =  _levelService.ProcessMessageXpAsync(userMessage); // Fire and forget
        }
        
        return Task.CompletedTask;
    }
}