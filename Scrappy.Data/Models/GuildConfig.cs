using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Scrappy.Data.Models;

public class GuildConfig
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public ulong GuildId { get; init; }

    // General settings
    [MaxLength(50)] public string? AppealLink { get; set; }

    // Logging channels
    public ulong? LogModerationEventChannelId { get; set; }
    public ulong? LogMessageEventChannelId { get; set; }

    // System channels
    public ulong? WelcomeChannelId { get; set; }
    public ulong? LevelUpChannelId { get; set; }

    // Command permissions
    public ulong? ModeratorRoleId { get; set; }
    public ulong? AdminRoleId { get; set; }
}