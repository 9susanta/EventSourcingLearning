using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Application.Abstractions
{
    public interface IAccountRepository
    {
        Task<AccountAggregate> GetByIdAsync(Guid accountId, CancellationToken cancellationToken);
        Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken);
        Task SaveWithSnapshotAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken);
    }
}
