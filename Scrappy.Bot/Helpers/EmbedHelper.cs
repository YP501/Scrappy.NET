using Discord;

namespace Scrappy.Bot.Helpers;

public static class EmbedHelper
{
    public static Embed CreateErrorEmbed(string description)
    {
        return new EmbedBuilder()
            .WithTitle($"❌ Error")
            .WithDescription(description)
            .WithColor(Color.Red)
            .Build();
    }

    public static Embed CreateCooldownEmbed(int cooldownTime)
    {
        return new EmbedBuilder()
            .WithTitle("⏳ Slow down!")
            .WithDescription($"You can only run a command every {cooldownTime} seconds")
            .WithColor(Color.LightOrange)
            .Build();
    }

    public static Embed CreateSuccessEmbed(string description)
    {
        return new EmbedBuilder()
            .WithTitle("✅ Success!")
            .WithDescription(description)
            .WithColor(Color.Green)
            .Build();
    }
}