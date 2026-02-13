using Scrappy.Data.Models;

namespace Scrappy.Data.Interfaces;

public interface IGuildConfigRepository
{
    Task<GuildConfig?> GetConfigAsync(ulong guildId);
    Task AddConfigAsync(GuildConfig config);
    Task SaveConfigAsync(GuildConfig config);
}