namespace EventSourcing.Bank.Infrastructure.Persistence
{
    using Microsoft.EntityFrameworkCore;

    public class EventStoreDbContext : DbContext
    {
        public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
        {
        }

        public DbSet<EventEntity> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EventEntity>(b =>
            {
                b.ToTable("BankAccountEvents");
                b.HasKey(e => e.Id);
                b.Property(e => e.Type).IsRequired();
                b.Property(e => e.Data).IsRequired();
                b.Property(e => e.OccurredAt).IsRequired();
                b.Property(e => e.Version).IsRequired();
                b.HasIndex(e => new { e.AggregateId, e.Version }).IsUnique();
            });
        }
    }
}
