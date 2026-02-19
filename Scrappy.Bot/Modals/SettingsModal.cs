using Discord.Interactions;

namespace Scrappy.Bot.Modals;

public class SettingsModal : IModal
{
    [InputLabel("Appeal link")]
    [RequiredInput(false)]
    [ModalTextInput("appeal_link", placeholder: "https://discord.gg/your_server", maxLength: 50)]
    public string? AppealLink { get; init; }

    public string Title => "Server Settings";


    // TODO: Implement this when Discord.net 3.19.0 drops
    // [InputLabel("Channel to log moderation events to")]
    // [RequiredInput(false)]
    // [ModalChannelSelect("moderation_log_channel_id")]
    // public IChannel? LogChannel { get; set; }
}