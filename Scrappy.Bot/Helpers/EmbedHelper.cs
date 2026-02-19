using Discord;

namespace Scrappy.Bot.Helpers;

public static class EmbedHelper
{
    public static Embed CreateErrorEmbed(string description)
    {
        return new EmbedBuilder()
            .WithDescription($"<:scrappy_cross:1029847563437887620> {description}")
            .WithColor(Color.Red)
            .Build();
    }

    public static Embed CreateSuccessEmbed(string description)
    {
        return new EmbedBuilder()
            .WithDescription($"<:scrappy_check:1029855559761002506> {description}")
            .WithColor(Color.Green)
            .Build();
    }

    public static Embed CreateWarningEmbed(string description)
    {
        return new EmbedBuilder()
            .WithDescription($"<:scrappy_warning:1029859720871292942> {description}")
            .WithColor(Color.Orange)
            .Build();
    }

    public static Embed CreateCooldownEmbed(int cooldownTime)
    {
        return new EmbedBuilder()
            .WithTitle("⏳ Slow down!")
            .WithDescription($"You can only run a command every {cooldownTime} seconds")
            .WithColor(Color.Orange)
            .Build();
    }
}