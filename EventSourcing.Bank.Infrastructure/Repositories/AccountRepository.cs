using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStore _eventStore;
        private readonly EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext _db;

        public AccountRepository(IEventStore eventStore, EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext db)
        {
            _eventStore = eventStore;
            _db = db;
        }

        public async Task<AccountAggregate> GetByIdAsync(Guid accountId, CancellationToken cancellationToken)
        {
            return await _eventStore.LoadAsync(accountId, cancellationToken);
        }

        public async Task<EventSourcing.Bank.Application.DTOs.AccountResponse> GetReadModelAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var readModel = await _db.AccountReadModels.FindAsync(new object[] { accountId }, cancellationToken);
            if (readModel == null) return null;
            
            return new EventSourcing.Bank.Application.DTOs.AccountResponse
            {
                Id = readModel.Id,
                AccountHolder = readModel.AccountHolder,
                Balance = readModel.Balance
            };
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
