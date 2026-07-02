using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Application.Abstractions
{
    public interface IEventStore
    {
        Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken);

        Task<AccountAggregate> LoadAsync(Guid accountId, CancellationToken cancellationToken);
        Task<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>> GetEventsAsync(Guid accountId, CancellationToken cancellationToken);
    }
}
