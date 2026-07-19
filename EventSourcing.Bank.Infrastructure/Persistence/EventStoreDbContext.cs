namespace EventSourcing.Bank.Infrastructure.Persistence
{
    using EventSourcing.Bank.Infrastructure.Persistence.ReadModels;
    using Microsoft.EntityFrameworkCore;

    public class EventStoreDbContext : DbContext
        {
            public EventStoreDbContext(DbContextOptions<EventStoreDbContext> options) : base(options)
            {
            }

            public DbSet<EventEntity> Events { get; set; }
            public DbSet<SnapshotEntity> Snapshots { get; set; }
            public DbSet<AccountReadModel> AccountReadModels { get; set; }
            public DbSet<OutboxMessage> OutboxMessages { get; set; }
            public DbSet<InboxMessage> InboxMessages { get; set; }

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

                modelBuilder.Entity<SnapshotEntity>(b =>
                {
                    b.ToTable("BankAccountSnapshots");
                    b.HasKey(e => e.Id);
                    b.Property(e => e.AggregateId).IsRequired();
                    b.Property(e => e.Version).IsRequired();
                    b.Property(e => e.AggregateType).IsRequired();
                    b.Property(e => e.Data).IsRequired();
                    b.Property(e => e.CreatedAt).IsRequired();
                    b.HasIndex(e => new { e.AggregateId, e.Version }).IsUnique();
                });

                modelBuilder.Entity<AccountReadModel>(b =>
                {
                    b.ToTable("AccountReadModels");
                    b.HasKey(e => e.Id);
                });

                modelBuilder.Entity<OutboxMessage>(b =>
                {
                    b.ToTable("OutboxMessages");
                    b.HasKey(e => e.Id);
                    b.Property(e => e.Type).IsRequired();
                    b.Property(e => e.Content).IsRequired();
                });

                modelBuilder.Entity<InboxMessage>(b =>
                {
                    b.ToTable("InboxMessages");
                    b.HasKey(e => e.Id);
                });
            }
        }
}
