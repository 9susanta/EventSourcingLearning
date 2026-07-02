using EventSourcing.Bank.Api.Events;

namespace EventSourcing.Bank.Api.Domain
{
    public class BankAccount
    {
        public Guid Id { get; private set; }

        public string AccountHolder { get; private set; }

        public decimal Balance { get; private set; }

        private readonly List<object> _uncommittedEvents = new();

        public IReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        // Called by callers to create a new account; emits AccountCreated and queues it as uncommitted
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

        // Called by callers to deposit money; emits MoneyDeposited, applies it and queues it
        public void Deposit(decimal amount)
        {
            var evt = new MoneyDeposited(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        // Called by callers to withdraw money; emits MoneyWithdrawn, applies it and queues it
        public void Withdraw(decimal amount)
        {
            var evt = new MoneyWithdrawn(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        // Invoked internally to apply AccountCreated during commands and replay
        private void Apply(AccountCreated evt)
        {
            Id = evt.AccountId;
            AccountHolder = evt.AccountHolder;
            Balance = 0;
        }

        // Invoked internally to apply MoneyDeposited during commands and replay
        private void Apply(MoneyDeposited evt)
        {
            Balance += evt.Amount;
        }

        // Invoked internally to apply MoneyWithdrawn during commands and replay
        private void Apply(MoneyWithdrawn evt)
        {
            Balance -= evt.Amount;
        }

        // Called by EventStore.Load to replay history and rehydrate the aggregate
        public void LoadFromHistory(IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                switch (e)
                {
                    case AccountCreated created:
                        Apply(created);
                        break;

                    case MoneyDeposited deposited:
                        Apply(deposited);
                        break;

                    case MoneyWithdrawn withdrawn:
                        Apply(withdrawn);
                        break;
                }
            }

            ClearEvents();
        }

        // Called by EventStore.Save and after replay to clear pending events
        public void ClearEvents()
        {
            _uncommittedEvents.Clear();
        }
    }

}
