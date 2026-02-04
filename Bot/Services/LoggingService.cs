using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;

namespace Bot.Services;

public class LoggingService
{
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interaction;
    public LoggingService(DiscordSocketClient client, InteractionService interaction)
    {
        _client = client;
        _interaction = interaction;
        
        _client.Log += LogAsync;
        _interaction.Log += LogAsync;
    }
    public Task LogAsync(LogMessage message)
    {
        if (message.Exception is CommandException cmdException)
        {
            Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
                              + $" failed to execute in {cmdException.Context.Channel}.");
            Console.WriteLine(cmdException);
        }
        else
            Console.WriteLine($"[General/{message.Severity}] {message}");

        return Task.CompletedTask;
    }
}