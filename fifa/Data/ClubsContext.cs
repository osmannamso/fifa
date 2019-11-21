using fifa.Models;
using Microsoft.EntityFrameworkCore;

namespace fifa.Data
{
    public class ClubsContext : DbContext
    {
        public ClubsContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<TopScorers> TopScorers { get; set; }
        public DbSet<TopSupport> TopSupports { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=fifa;Username=root;Password=root");
        }
    }
}