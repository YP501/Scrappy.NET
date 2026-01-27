using Discord.WebSocket;

public class MessageRecievedHandler
{
    private readonly DiscordSocketClient _client;
    public MessageRecievedHandler(DiscordSocketClient client)
    {
        _client = client;
        client.MessageReceived += OnMessageRecieved;
    }
    private async Task OnMessageRecieved(SocketMessage msg)
    {
        Console.WriteLine(msg.Content);
    }
}