using Discord;
using Discord.Interactions;
using Discord.Net;
using Scrappy.Bot.Attributes;
using Scrappy.Bot.Helpers;
using Scrappy.Bot.Services;
using Scrappy.Data.Enums;

namespace Scrappy.Bot.Commands.Administration;

[Group("purge", "Mass-delete messages in a channel")]
[RequireMinimumPermission(PermissionLevel.Administrator)]
[CommandContextType(InteractionContextType.Guild)]
public class PurgeCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GuildConfigService _guildConfigService;

    public PurgeCommand(GuildConfigService guildConfigService)
    {
        _guildConfigService = guildConfigService;
    }

    private async Task ExecutePurge(IEnumerable<IMessage> messages, string type)
    {
        await DeferAsync(true);

        if (Context.Channel is not ITextChannel channel)
        {
            await FollowupAsync(embed: EmbedHelper.CreateErrorEmbed(
                "Not a valid channel to purge in. Try executing this command in a server text channel."));
            return;
        }

        var validMessages = messages
            .Where(m => m.Timestamp > DateTimeOffset.Now.AddDays(-14))
            .ToList();

        if (validMessages.Count == 0)
        {
            await FollowupAsync(
                embed: EmbedHelper.CreateErrorEmbed("No matching messages found that are younger than 14 days."));
            return;
        }

        try
        {
            await channel.DeleteMessagesAsync(validMessages);
        }
        catch (HttpException e) when (e.DiscordCode is DiscordErrorCode.MissingPermissions)
        {
            await FollowupAsync(
                embed: EmbedHelper.CreateErrorEmbed("I don't have permission to delete messages here."));
            return;
        }

        await FollowupAsync(
            embed: EmbedHelper.CreateSuccessEmbed(
                $"Purged {validMessages.Count} message{(validMessages.Count > 1 ? "s" : "")}"));

        var config = await _guildConfigService.GetOrAddConfigAsync(Context.Guild.Id);
        if (!config.LogModerationEventChannelId.HasValue) return;

        var logChannel = Context.Guild.GetTextChannel(config.LogModerationEventChannelId.Value);
        if (logChannel == null) return;

        var embed = new EmbedBuilder()
            .WithTitle("Messages Purged")
            .WithColor(Color.Purple)
            .AddField("Channel", channel.Mention, true)
            .AddField("Moderator", Context.User.Mention, true)
            .AddField("Amount", validMessages.Count, true)
            .AddField("Type", type)
            .WithCurrentTimestamp()
            .Build();

        await logChannel.SendMessageAsync(embed: embed);
        // TODO: maybe add confirmation buttons
    }

    private async Task<IEnumerable<IMessage>> FetchFilteredMessages(int searchAmount, Func<IMessage, bool> filter)
    {
        var messages = await Context.Channel.GetMessagesAsync(searchAmount).FlattenAsync();
        return messages.Where(filter).Take(searchAmount);
    }

    [SlashCommand("any", "Purge any message of all types")]
    public async Task PurgeAny([MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(await FetchFilteredMessages(searchAmount, _ => true), "any");
    }

    [SlashCommand("bots", "Purge messages sent by bots")]
    public async Task PurgeBots([MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(await FetchFilteredMessages(searchAmount, m => m.Author.IsBot), "bots");
    }

    [SlashCommand("contains", "Purge messages containing a piece of text")]
    public async Task PurgeContain(string text, [MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(
            await FetchFilteredMessages(searchAmount,
                m => m.Content.Contains(text, StringComparison.OrdinalIgnoreCase)), $"contains: {text}");
    }

    [SlashCommand("excludes", "Purge messages excluding a piece of text")]
    public async Task PurgeExcludes(string text, [MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(
            await FetchFilteredMessages(searchAmount,
                m => !m.Content.Contains(text, StringComparison.OrdinalIgnoreCase)), $"excludes: {text}");
    }

    [SlashCommand("embeds", "Purge messages with embeds")]
    public async Task PurgeEmbeds([MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(await FetchFilteredMessages(searchAmount, m => m.Embeds.Count > 0), "embeds");
    }

    [SlashCommand("startswith", "Purge messages starting with text")]
    public async Task PurgeStartsWith(string text, [MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(
            await FetchFilteredMessages(searchAmount,
                m => m.Content.StartsWith(text, StringComparison.OrdinalIgnoreCase)), $"startswith: {text}");
    }

    [SlashCommand("endswith", "Purge messages ending with text")]
    public async Task PurgeEndsWith(string text, [MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(
            await FetchFilteredMessages(searchAmount,
                m => m.Content.EndsWith(text, StringComparison.OrdinalIgnoreCase)), $"endswith: {text}");
    }

    [SlashCommand("attachments", "Purge messages with attachments")]
    public async Task PurgeAttachments([MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(await FetchFilteredMessages(searchAmount, m => m.Attachments.Count > 0), "attachments");
    }

    [SlashCommand("user", "Purge messages of a specific user")]
    public async Task PurgeUser(IUser user, [MinValue(1)] [MaxValue(100)] int searchAmount)
    {
        await ExecutePurge(await FetchFilteredMessages(searchAmount, m => m.Author.Id == user.Id),
            $"user: {user.Mention}");
    }

    [MessageCommand("Purge To Here")]
    public async Task PurgeUntilHere(IMessage targetMessage)
    {
        // Overwrite FetchFilteredMessages with own logic
        List<IMessage> allMessages = [targetMessage];
        var pagedMessages = Context.Channel.GetMessagesAsync(targetMessage, Direction.After);
        await foreach (var page in pagedMessages)
        foreach (var msg in page)
        {
            if (msg.Timestamp < DateTimeOffset.Now.AddDays(-14)) goto EndLoop;
            allMessages.Add(msg);

            // Hard limit to prevent fetching too many messages and overloading the api
            if (allMessages.Count >= 1000) break;
        }

        EndLoop:
        await ExecutePurge(allMessages, "until here");
    }
}