using Discord;
using Discord.Interactions;

namespace Bot.Commands.General;

public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Check the bot's latency")]
    public async Task ExecuteAsync()
    {
        int latency = Context.Client.Latency;

        var embed = new EmbedBuilder()
            .WithTitle("Pong! 🏓")
            .WithDescription($"The bot's latency is **{latency}ms**")
            .WithColor(Color.Purple)
            .WithCurrentTimestamp()
            .Build();
        
        await RespondAsync(embed: embed);
    }
}