using Discord.WebSocket;

public class ReadyHandler
{
    private readonly DiscordSocketClient _client;
    public ReadyHandler(DiscordSocketClient client)
    {
        _client = client;
        _client.Ready += OnReadyAsync;
    }
    private Task OnReadyAsync()
    {
        // Example action on Ready event
        // Console.WriteLine($"Bot is connected as {_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator}");
        return Task.CompletedTask;
    }
}