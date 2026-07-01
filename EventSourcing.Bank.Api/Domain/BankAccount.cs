using EventSourcing.Bank.Api.Events;

namespace EventSourcing.Bank.Api.Domain
{
    public class BankAccount
    {
        public Guid Id { get; private set; }

        public string AccountHolder { get; private set; }

        public decimal Balance { get; private set; }

        private readonly List<object> _uncommittedEvents = new();

        public static BankAccount Create(string accountHolder)
        {
            var account = new BankAccount();

            var evt = new AccountCreated(
                Guid.NewGuid(),
                accountHolder);

            account.Apply(evt);

            account._uncommittedEvents.Add(evt);

            return account;
        }

        public void Deposit(decimal amount)
        {
            var evt = new MoneyDeposited(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        public void Withdraw(decimal amount)
        {
            var evt = new MoneyWithdrawn(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        private void Apply(AccountCreated evt)
        {
            Id = evt.AccountId;
            AccountHolder = evt.AccountHolder;
            Balance = 0;
        }

        private void Apply(MoneyDeposited evt)
        {
            Balance += evt.Amount;
        }

        private void Apply(MoneyWithdrawn evt)
        {
            Balance -= evt.Amount;
        }
    }
}
