using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Interfaces;
using Scrappy.Data.Models;

namespace Scrappy.Data.Repositories;

public class InfractionRepository : IInfractionRepository
{
    private readonly ScrappyDbContext _dbContext;
    public InfractionRepository(ScrappyDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task AddInfractionAsync(Infraction infraction)
    {
        _dbContext.Infractions.Add(infraction);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<Infraction>> GetUserInfractionsAsync(ulong guildId, ulong targetId)
    {
        var infractions = await _dbContext.Infractions
            .Where(i => i.GuildId == guildId && i.TargetId == targetId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return infractions;
    }

    public async Task<Infraction?> GetInfractionByCaseIdAsync(ulong guildId, string caseId)
    {
        var infraction = await _dbContext.Infractions
            .FirstOrDefaultAsync(i => i.GuildId == guildId && i.CaseId == caseId);
        
        return infraction;
    }
}