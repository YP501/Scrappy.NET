using Discord;
using Discord.Interactions;

namespace Scrappy.Bot.Modals;

public class SettingsModal : IModal
{
    public string Title => "Server Settings";

    [RequiredInput(false)]
    [InputLabel("Appeal link")]
    [ModalTextInput("appeal_link", placeholder: "https://discord.gg/your_server", maxLength: 50)]
    public string? AppealLink { get; init; }

    
    [RequiredInput(false)]
    [InputLabel("Moderation actions log channel")]
    [ModalChannelSelect("moderation_log_channel_id")]
    public IChannel? LogChannel { get; set; }
}