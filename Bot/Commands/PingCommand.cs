using Discord;
using Discord.Interactions;

namespace Bot.Commands;

public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check the bot's latency")]
    public async Task HandleCommand()
    {
        var latency = Context.Client.Latency;

        var embed = new EmbedBuilder()
            .WithTitle("Pong! 🏓")
            .WithDescription($"The bot latency is **{latency}ms**")
            .WithColor(Color.Purple)
            .WithCurrentTimestamp()
            .Build();
        
        await RespondAsync(embed: embed);
    }
}