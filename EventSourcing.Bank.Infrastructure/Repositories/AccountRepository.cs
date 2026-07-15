using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStore _eventStore;
        private readonly EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext _db;
        private readonly EventSourcing.Bank.Application.Services.DomainEventDispatcher _dispatcher;

        public AccountRepository(
            IEventStore eventStore, 
            EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext db,
            EventSourcing.Bank.Application.Services.DomainEventDispatcher dispatcher)
        {
            _eventStore = eventStore;
            _db = db;
            _dispatcher = dispatcher;
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
            var domainEventsToDispatch = account.DomainEvents.ToList();

            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);
            
            foreach (var evt in domainEventsToDispatch)
            {
                if (evt is EventSourcing.Bank.Domain.Events.IDomainEvent domainEvt)
                {
                    await _dispatcher.DispatchAsync(domainEvt);
                }
            }
            account.ClearDomainEvents();
        }

        public async Task SaveWithSnapshotAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            var domainEventsToDispatch = account.DomainEvents.ToList();

            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);

            // Cast to SqlServerEventStore to access snapshot functionality
            if (_eventStore is EventSourcing.Bank.Infrastructure.Persistence.SqlServerEventStore sqlEventStore)
            {
                await sqlEventStore.SaveSnapshotAsync(account, cancellationToken);
            }

            foreach (var evt in domainEventsToDispatch)
            {
                if (evt is EventSourcing.Bank.Domain.Events.IDomainEvent domainEvt)
                {
                    await _dispatcher.DispatchAsync(domainEvt);
                }
            }
            account.ClearDomainEvents();
        }
    }
}
