using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Enums;

namespace Scrappy.Data.Models;

[Index(nameof(TargetId))]
[Index(nameof(GuildId))]
[Index(nameof(GuildId), nameof(CaseId),IsUnique = true)]
public class Infraction
{
    [Key]
    public long Id { get; init; }
    
    public ulong GuildId { get; init; }
    
    public ulong TargetId { get; init; }
    
    public ulong IssuerId { get; init; }
    
    [MaxLength(500)]
    public string Reason { get; init; } = "No reason provided";
    
    public InfractionType Type { get; init; }
    
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    
    [MaxLength(8)]
    public string CaseId { get; init; } = Guid.NewGuid().ToString("N")[..8];
}