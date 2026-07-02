using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;

namespace EventSourcing.Bank.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IEventStore _eventStore;

        public AccountService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<AccountAggregate> CreateAccountAsync(string accountHolder, CancellationToken cancellationToken)
        {
            var account = AccountAggregate.Create(accountHolder);

            await _eventStore.SaveAsync(account, 0, cancellationToken);

            return account;
        }

        public async Task<AccountAggregate> DepositAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var account = await _eventStore.LoadAsync(accountId, cancellationToken);

            account.Deposit(amount);

            await _eventStore.SaveAsync(account, account.Version, cancellationToken);

            return account;
        }

        public async Task<AccountAggregate> WithdrawAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var account = await _eventStore.LoadAsync(accountId, cancellationToken);

            account.Withdraw(amount);

            await _eventStore.SaveAsync(account, account.Version, cancellationToken);

            return account;
        }

        public Task<AccountAggregate> GetAsync(Guid accountId, CancellationToken cancellationToken)
        {
            return _eventStore.LoadAsync(accountId, cancellationToken);
        }
        public Task<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>> GetEventsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            return _eventStore.GetEventsAsync(accountId, cancellationToken);
        }
    }
}
