using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    public class SqlServerEventStore : IEventStore
    {
        private readonly EventStoreDbContext _db;

        public SqlServerEventStore(EventStoreDbContext db)
        {
            _db = db;
            _db.Database.EnsureCreated();
        }

        public async Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            var events = account.UncommittedEvents;
            if (events == null || !events.Any()) return;

            var currentVersion = await _db.Events.Where(e => e.AggregateId == account.Id).MaxAsync(e => (int?)e.Version, cancellationToken) ?? 0;
            if (currentVersion != expectedVersion)
            {
                throw new EventSourcing.Bank.Application.Abstractions.ConcurrencyException("Concurrency conflict: expected version does not match current stream version.");
            }

            var nextVersion = currentVersion;
            foreach (var evt in events)
            {
                cancellationToken.ThrowIfCancellationRequested();
                nextVersion++;

                var entity = new EventEntity
                {
                    Id = Guid.NewGuid(),
                    AggregateId = account.Id,
                    Type = evt.GetType().FullName ?? evt.GetType().Name,
                    Data = EventSerializer.Serialize(evt),
                    OccurredAt = DateTime.UtcNow,
                    Version = nextVersion
                };

                _db.Events.Add(entity);
            }

            await _db.SaveChangesAsync(cancellationToken);

            account.SetVersion(nextVersion);
            account.ClearEvents();
        }

        public async Task<AccountAggregate> LoadAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var events = new List<object>();

            var rows = await _db.Events.Where(e => e.AggregateId == accountId).OrderBy(e => e.Version).ToListAsync(cancellationToken);
            foreach (var row in rows)
            {
                var typeName = row.Type;
                var data = row.Data;

                var type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(t => t.FullName == typeName);
                if (type == null) continue;

                var evt = EventSerializer.Deserialize(data, type);
                if (evt != null) events.Add(evt);
            }

            var account = new AccountAggregate();
            account.LoadFromHistory(events);
            return account;
        }

        public async Task<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>> GetEventsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var rows = await _db.Events.Where(e => e.AggregateId == accountId).OrderBy(e => e.Version).ToListAsync(cancellationToken);
            return rows.Select(r => new EventSourcing.Bank.Application.DTOs.EventDto
            {
                Type = r.Type,
                Data = r.Data,
                Version = r.Version,
                OccurredAt = r.OccurredAt
            });
        }
    }
}
