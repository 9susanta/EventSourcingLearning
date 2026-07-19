using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IEventStore _eventStore;
        private readonly EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext _db;
        private readonly MediatR.IPublisher _publisher;

        public AccountRepository(
            IEventStore eventStore, 
            EventSourcing.Bank.Infrastructure.Persistence.EventStoreDbContext db,
            MediatR.IPublisher publisher)
        {
            _eventStore = eventStore;
            _db = db;
            _publisher = publisher;
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

            // 2. THE OUTBOX PATTERN
            // We serialize the intent to publish and save it BEFORE calling the event store.
            // When the EventStore calls SaveChangesAsync, it will atomically commit the Events and the OutboxMessages together.
            foreach (var evt in domainEventsToDispatch)
            {
                if (evt is EventSourcing.Bank.Domain.Events.IDomainEvent domainEvt)
                {
                    var outboxMessage = new EventSourcing.Bank.Infrastructure.Persistence.OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = domainEvt.GetType().AssemblyQualifiedName ?? domainEvt.GetType().Name,
                        Content = System.Text.Json.JsonSerializer.Serialize(domainEvt, domainEvt.GetType()),
                        OccurredOn = DateTime.UtcNow
                    };
                    
                    _db.OutboxMessages.Add(outboxMessage);
                }
            }
            
            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);
            

            account.ClearDomainEvents();
        }

        public async Task SaveWithSnapshotAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            var domainEventsToDispatch = account.DomainEvents.ToList();

            // 2. THE OUTBOX PATTERN
            // We serialize the intent to publish and save it BEFORE calling the event store.
            foreach (var evt in domainEventsToDispatch)
            {
                if (evt is EventSourcing.Bank.Domain.Events.IDomainEvent domainEvt)
                {
                    var outboxMessage = new EventSourcing.Bank.Infrastructure.Persistence.OutboxMessage
                    {
                        Id = Guid.NewGuid(),
                        Type = domainEvt.GetType().AssemblyQualifiedName ?? domainEvt.GetType().Name,
                        Content = System.Text.Json.JsonSerializer.Serialize(domainEvt, domainEvt.GetType()),
                        OccurredOn = DateTime.UtcNow
                    };
                    
                    _db.OutboxMessages.Add(outboxMessage);
                }
            }

            await _eventStore.SaveAsync(account, expectedVersion, cancellationToken);

            // Cast to SqlServerEventStore to access snapshot functionality
            if (_eventStore is EventSourcing.Bank.Infrastructure.Persistence.SqlServerEventStore sqlEventStore)
            {
                await sqlEventStore.SaveSnapshotAsync(account, cancellationToken);
            }

            account.ClearDomainEvents();
        }
    }
}
