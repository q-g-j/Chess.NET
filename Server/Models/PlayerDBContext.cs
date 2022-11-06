using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Server.Models
{
    public class ChessDBContext : DbContext
    {
        public ChessDBContext(DbContextOptions<ChessDBContext> options)
            : base(options) { }

        public DbSet<Player> Players => Set<Player>();
        public DbSet<Invitation> Invitations => Set<Invitation>();
        public DbSet<Game> Games => Set<Game>();

        // suppress info messages:
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ContextInitialized));
        }
    }
}
