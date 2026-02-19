using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Enums;

namespace Scrappy.Data.Models;

[Index(nameof(GuildId), nameof(TargetId))] // Index user search
[Index(nameof(GuildId), nameof(IssuerId))] // Index moderator search
[Index(nameof(GuildId), nameof(CaseId), IsUnique = true)] // CaseId duplicates allowed if not same server 
public class Infraction
{
    [Key] public long Id { get; init; }

    public ulong GuildId { get; init; }

    // Foreign key to GuildConfig
    [ForeignKey(nameof(GuildId))] public virtual GuildConfig Guild { get; init; } = null!;

    public ulong TargetId { get; init; }

    public ulong IssuerId { get; init; }

    [MaxLength(500)] public string Reason { get; init; } = "No reason provided";

    public InfractionType Type { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    [MaxLength(8)] public string CaseId { get; init; } = Guid.NewGuid().ToString("N")[..8];
}