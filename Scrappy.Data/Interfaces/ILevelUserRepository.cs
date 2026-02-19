using Scrappy.Data.Models;

namespace Scrappy.Data.Interfaces;

public interface ILevelUserRepository
{
    Task<List<LevelUser>> GetTopUsersAsync(ulong guildId, int limit);
    Task<LevelUser?> GetLevelUserAsync(ulong guildId, ulong userId);
    Task AddLevelUserAsync(LevelUser user);
    Task UpdateLevelUser(LevelUser user);
}