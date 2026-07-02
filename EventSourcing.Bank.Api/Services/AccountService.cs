using EventSourcing.Bank.Api.Domain;
using EventSourcing.Bank.Api.Store;

namespace EventSourcing.Bank.Api.Services
{
    public class AccountService
    {
        private readonly IEventStore _eventStore;

        public AccountService(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public BankAccount CreateAccount(string accountHolder)
        {
            var account = BankAccount.Create(accountHolder);

            _eventStore.Save(account);

            return account;
        }
        public BankAccount Deposit(Guid accountId, decimal amount)
        {
            var account = _eventStore.Load(accountId);

            account.Deposit(amount);

            _eventStore.Save(account);

            return account;
        }

        public BankAccount Withdraw(Guid accountId, decimal amount)
        {
            var account = _eventStore.Load(accountId);

            account.Withdraw(amount);

            _eventStore.Save(account);

            return account;
        }
        public BankAccount Get(Guid accountId)
        {
            return _eventStore.Load(accountId);
        }

    }
}
