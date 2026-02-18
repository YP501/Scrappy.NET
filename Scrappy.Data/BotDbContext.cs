using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Models;

namespace Scrappy.Data;

public class BotDbContext : DbContext
{
    public DbSet<Infraction> Infractions { get; set; }
    public DbSet<GuildConfig> GuildConfigs { get; set; }
    public DbSet<LevelUser> LevelUsers { get; set; }
    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }
}