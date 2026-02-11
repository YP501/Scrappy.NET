using Microsoft.EntityFrameworkCore;
using Scrappy.Data.Models;

namespace Scrappy.Data;

public class ScrappyDbContext : DbContext
{
    public DbSet<Infraction> Infractions { get; set; }
    public ScrappyDbContext(DbContextOptions<ScrappyDbContext> options) : base(options) { }
}