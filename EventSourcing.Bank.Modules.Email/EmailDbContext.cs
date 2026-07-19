using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Modules.Email
{
    public class EmailDbContext : DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }

        public DbSet<InboxMessage> InboxMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Email");
            
            modelBuilder.Entity<InboxMessage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedNever();
            });
        }
    }
}
