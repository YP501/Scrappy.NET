using Scrappy.Data.Models;

namespace Scrappy.Data.Interfaces;

public interface IInfractionRepository
{
    Task AddInfractionAsync(Infraction infraction);
    Task<List<Infraction>> GetUserInfractionsAsync(ulong guildId, ulong targetId);
    Task<Infraction?> GetInfractionByCaseIdAsync(ulong guildId, string caseId);
}