using Discord;
using Discord.Interactions;
using Scrappy.Bot.Attributes;
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
    [RequireMinimumPermission(PermissionLevel.ServerOwner)]
    public async Task ExecuteAsync()
    {
        // TODO: Remove this and implement correct [ModalChannelSelect] attribute when Discord.net stable 3.19.0 drops
        await RespondAsync("Not implemented!");
        return;

        var config = await _configService.GetOrAddConfigAsync(Context.Guild.Id);

        // Pre-populate settings modal with already set settings
        var modal = new SettingsModal
        {
            AppealLink = config.AppealLink
        };

        await Context.Interaction.RespondWithModalAsync<SettingsModal>("settings_modal", modal);
    }


    // React to modal
    [ModalInteraction("settings_modal")]
    public async Task HandleAsync(SettingsModal modal)
    {
        await DeferAsync(ephemeral: true);

        var config = await _configService.GetOrAddConfigAsync(Context.Guild.Id);
        // Update config values
        config.AppealLink = string.IsNullOrWhiteSpace(modal.AppealLink) ? null : modal.AppealLink;

        await _configService.UpdateConfigAsync(config);
        await FollowupAsync("Settings saved successfully!"); // TODO: Embed this
    }
}