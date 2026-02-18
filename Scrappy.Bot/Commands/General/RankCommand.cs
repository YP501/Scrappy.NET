using Discord;
using Discord.Interactions;
using Scrappy.Bot.Helpers;
using Scrappy.Bot.Services;

namespace Scrappy.Bot.Commands.General;

[Group("rank", "All about user level rankings")]
public class RankCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly LevelService _levelService;

    public RankCommand(LevelService levelService)
    {
        _levelService = levelService;
    }

    [SlashCommand("show", "View your own rank or the one of someone else")]
    public async Task Show(
        [Summary("user", "The user you want to see the level of")]
        IUser? user = null)
    {
        var targetUser = user ?? Context.User; // Default to yourself when no user provided

        var stats = await _levelService.GetLevelUserAsync(Context.Guild.Id, targetUser.Id);
        if (stats == null)
        {
            await RespondAsync(
                embed: EmbedHelper.CreateWarningEmbed($"{(targetUser.Id == Context.User.Id ? "You haven't" : $"{targetUser.Mention} hasn't")} earned any xp yet"),
                ephemeral: true);
            return;
        }

        int currentLevel = stats.CurrentLevel;
        long currentXp = stats.TotalXp;

        long minXp = LevelHelper.GetRequiredXpForLevel(currentLevel);
        long maxXp = LevelHelper.GetRequiredXpForLevel(currentLevel + 1);
        string progressBar = LevelHelper.GetProgressBar(currentXp, currentLevel, 15);

        var embed = new EmbedBuilder()
            .WithTitle($"Rank Card - {targetUser.GlobalName ?? targetUser.Username}")
            .WithThumbnailUrl(targetUser.GetAvatarUrl(ImageFormat.Jpeg))
            .AddField("Level", currentLevel, true)
            .AddField("Total XP", currentXp, true)
            .AddField($"Progress to level {currentLevel + 1}",
                $"**{currentXp - minXp}** / **{maxXp - minXp}** XP\n{progressBar}", true)
            .WithColor(Color.Purple)
            .Build();

        await RespondAsync(embed: embed);
    }

    [SlashCommand("leaderboard", "View the top ranking users of this server")]
    public async Task Leaderboard(
        [Summary("limit", "How many users do you want to see?")]
        [MinValue(1)] [MaxValue(25)] int limit = 10)
    {
        var topUsers = await _levelService.GetTopUsersAsync(Context.Guild.Id, 10);

        if (!topUsers.Any())
        {
            await RespondAsync(embed: EmbedHelper.CreateWarningEmbed("No one has earned any XP yet. Start chatting!"),
                ephemeral: true);
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle($"🏆 Leaderboard - {Context.Guild.Name}")
            .WithThumbnailUrl(Context.Guild.IconUrl)
            .WithColor(Color.Purple);

        string description = "";
        for (int i = 0; i < topUsers.Count; i++)
        {
            var user = topUsers[i];
            var discordUser = Context.Guild.GetUser(user.UserId);
            string name = discordUser?.GlobalName ?? discordUser?.Username ?? "Unknown User";

            string prefix = i switch
            {
                0 => "🥇",
                1 => "🥈",
                2 => "🥉",
                _ => $"**{i + 1}.**"
            };

            int level = LevelHelper.CalculateLevelForXp(user.TotalXp);
            description += $"{prefix} {name} - Level {level} ({user.TotalXp} XP)\n";
        }

        embed.WithDescription(description);
        await RespondAsync(embed: embed.Build());
    }
}