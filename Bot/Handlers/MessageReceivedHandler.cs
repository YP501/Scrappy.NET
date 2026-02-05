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
        if (msg.Author.Id == _client.CurrentUser.Id) return  Task.CompletedTask; // Stop if its own message
        
        Console.WriteLine(msg.CleanContent);
        return Task.CompletedTask;
    }
}