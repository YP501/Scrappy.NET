using Discord;
using Discord.Interactions;
using Scrappy.Bot.Attributes;
using Scrappy.Bot.Helpers;
using Scrappy.Bot.Modals;
using Scrappy.Bot.Services;
using Scrappy.Data.Enums;

namespace Scrappy.Bot.Commands.Administration;

public class SettingsCommand : InteractionModuleBase<SocketInteractionContext>
{
    private readonly GuildConfigService _configService;

    public SettingsCommand(GuildConfigService configService)
    {
        _configService = configService;
    }

    // Reply with modal
    [SlashCommand("settings", "Change the settings of the bot for your server")]
    [CommandContextType(InteractionContextType.Guild)]
    [RequireMinimumPermission(PermissionLevel.BotDeveloper)]
    public async Task ExecuteAsync()
    {
        var config = await _configService.GetOrAddConfigAsync(Context.Guild.Id);

        // Fetch stuff needed with just the ulong ids

        // Pre-populate settings modal with already set settings
        var modal = new SettingsModal
        {
            AppealLink = config.AppealLink,
            LogChannel = config.LogModerationEventChannelId.HasValue
                ? Context.Guild.GetChannel(config.LogModerationEventChannelId.Value)
                : null,
        };

        await Context.Interaction.RespondWithModalAsync("settings_modal", modal);
    }


    // React to modal
    [ModalInteraction("settings_modal")]
    public async Task HandleAsync(SettingsModal modal)
    {
        await DeferAsync(true);

        // Verify modal response fields
        if (modal.LogChannel is not ITextChannel)
        {
            await FollowupAsync(embed: EmbedHelper.CreateErrorEmbed("Please select a TEXT channel for the moderation logs."));
            return;
        }
        
        // Update config values
        var config = await _configService.GetOrAddConfigAsync(Context.Guild.Id);
        config.AppealLink = string.IsNullOrWhiteSpace(modal.AppealLink) ? null : modal.AppealLink;
        config.LogModerationEventChannelId = modal.LogChannel?.Id;

        await _configService.UpdateConfigAsync(config);

        var successEmbed = EmbedHelper.CreateSuccessEmbed("Settings updated!");
        await FollowupAsync(embed: successEmbed, ephemeral: true);
    }
}