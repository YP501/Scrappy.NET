using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Data.Repositories;

public class GuildConfigRepository : IGuildConfigRepository
{
    private readonly BotDbContext _dbContext;

    public GuildConfigRepository(BotDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GuildConfig?> GetConfigAsync(ulong guildId)
    {
        return await _dbContext.GuildConfigs.FindAsync(guildId);
    }

    public async Task AddConfigAsync(GuildConfig config)
    {
        _dbContext.GuildConfigs.Add(config);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateConfigAsync(GuildConfig config)
    {
        _dbContext.GuildConfigs.Update(config);
        await _dbContext.SaveChangesAsync();
    }
}