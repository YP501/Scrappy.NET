using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Data.Repositories;

public class LevelUserRepository : ILevelUserRepository
{
    private readonly BotDbContext _dbContext;

    public LevelUserRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<LevelUser>> GetTopUsersAsync(ulong guildId, int limit)
    {
        return await _dbContext.LevelUsers
            .Where(u => u.GuildId == guildId)
            .OrderByDescending(u => u.TotalXp)
            .Take(limit)
            .AsNoTracking() // Faster for read-only lists
            .ToListAsync();
    }

    public async Task<LevelUser?> GetLevelUserAsync(ulong guildId, ulong userId)
    {
        return await _dbContext.LevelUsers.FirstOrDefaultAsync(u => u.GuildId == guildId && u.UserId == userId);
    }

    public async Task AddLevelUserAsync(LevelUser user)
    {
        _dbContext.LevelUsers.Add(user);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateLevelUser(LevelUser user)
    {
        _dbContext.LevelUsers.Update(user);
        await _dbContext.SaveChangesAsync();
    }
}