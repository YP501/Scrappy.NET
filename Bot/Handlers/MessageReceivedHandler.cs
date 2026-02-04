using Bot.Interfaces;
using Discord;
using Discord.WebSocket;

namespace Bot.Handlers;
public class MessageReceivedHandler : IEventHandler
{
    private readonly DiscordSocketClient _client;
    public MessageReceivedHandler(DiscordSocketClient client)
    {
        _client = client;
    }

    public Task InitializeAsync()
    {
        _client.MessageReceived += OnMessageReceived;
        return Task.CompletedTask;
    }

    private Task OnMessageReceived(SocketMessage msg)
    {
        Console.WriteLine(msg.CleanContent);
        return Task.CompletedTask;
    }
}