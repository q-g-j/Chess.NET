using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Server.Models
{
    public class PlayerDBContext : DbContext
    {
        public PlayerDBContext(DbContextOptions<PlayerDBContext> options)
            : base(options) { }

        public DbSet<Player> Player => Set<Player>();

        // suppress info messages:
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ContextInitialized));
        }
    }
}
