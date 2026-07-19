using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Modules.Security
{
    public class SecurityDbContext : DbContext
    {
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options) { }

        public DbSet<InboxMessage> InboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Security");
            
            modelBuilder.Entity<InboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
