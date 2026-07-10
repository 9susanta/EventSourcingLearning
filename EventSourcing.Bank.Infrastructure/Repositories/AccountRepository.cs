using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStore _eventStore;

        public AccountRepository(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<AccountAggregate> GetByIdAsync(Guid accountId, CancellationToken cancellationToken)
        {
            return await _eventStore.LoadAsync(accountId, cancellationToken);
        }

        public async Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);
        }

        public async Task SaveWithSnapshotAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);

            // Cast to SqlServerEventStore to access snapshot functionality
            if (_eventStore is EventSourcing.Bank.Infrastructure.Persistence.SqlServerEventStore sqlEventStore)
            {
                await sqlEventStore.SaveSnapshotAsync(account, cancellationToken);
            }
        }
    }
}
