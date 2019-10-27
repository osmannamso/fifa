using fifa.Models;
using Microsoft.EntityFrameworkCore;

namespace fifa.Data
{
    public class ClubsContext : DbContext
    {
        public ClubsContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=fifa;Username=root;Password=root");
        }
    }
}