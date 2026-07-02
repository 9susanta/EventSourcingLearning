using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<Guid, List<object>> _store = new();

        public Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            if (!_store.ContainsKey(account.Id))
            {
                _store[account.Id] = new List<object>();
            }

            var currentVersion = _store[account.Id].Count;
            if (expectedVersion != currentVersion)
            {
                throw new EventSourcing.Bank.Application.Abstractions.ConcurrencyException("Concurrency conflict: expected version does not match current stream version.");
            }

            _store[account.Id].AddRange(account.UncommittedEvents);

            account.SetVersion(_store[account.Id].Count);
            account.ClearEvents();

            return Task.CompletedTask;
        }

        public Task<AccountAggregate> LoadAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var account = new AccountAggregate();

            if (_store.TryGetValue(accountId, out var events))
            {
                account.LoadFromHistory(events);
            }

            return Task.FromResult(account);
        }

        public Task<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>> GetEventsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            if (!_store.TryGetValue(accountId, out var events))
            {
                return Task.FromResult(Enumerable.Empty<EventSourcing.Bank.Application.DTOs.EventDto>());
            }

            var list = events.Select((e, idx) => new EventSourcing.Bank.Application.DTOs.EventDto
            {
                Type = e.GetType().FullName ?? e.GetType().Name,
                Data = EventSourcing.Bank.Infrastructure.Serialization.EventSerializer.Serialize(e),
                Version = idx + 1,
                OccurredAt = DateTime.MinValue
            });

            return Task.FromResult(list);
        }
    }
}
