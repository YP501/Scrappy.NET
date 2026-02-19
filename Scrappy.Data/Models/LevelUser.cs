using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Scrappy.Data.Models;

[Index(nameof(GuildId), nameof(UserId), IsUnique = true)] // For fast user querying )]
public class LevelUser
{
    [Key] public ulong Id { get; init; }

    public ulong GuildId { get; set; }

    [ForeignKey(nameof(GuildId))] public virtual GuildConfig GuildConfig { get; set; } = null!;

    public ulong UserId { get; set; }
    public long TotalXp { get; set; }
}