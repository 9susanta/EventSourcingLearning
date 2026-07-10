using System.Collections.Generic;
using System.Linq;
using EventSourcing.Bank.Domain.Events;

namespace EventSourcing.Bank.Domain.Aggregates
{
    public class AccountAggregate
    {
        public int Version { get; private set; }
        public Guid Id { get; private set; }

        public string AccountHolder { get; private set; }

        public decimal Balance { get; private set; }

        private readonly List<object> _uncommittedEvents = new();

        public IReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        public static AccountAggregate Create(string accountHolder)
        {
            var account = new AccountAggregate();

            var evt = new AccountCreatedEvent(
                Guid.NewGuid(),
                accountHolder);

            account.Apply(evt);

            account._uncommittedEvents.Add(evt);

            return account;
        }

        public void Deposit(decimal amount)
        {
            var evt = new MoneyDepositedEvent(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        public void Withdraw(decimal amount)
        {
            var evt = new MoneyWithdrawnEvent(amount);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        private void Apply(AccountCreatedEvent evt)
        {
            Id = evt.AccountId;
            AccountHolder = evt.AccountHolder;
            Balance = 0;
        }

        private void Apply(MoneyDepositedEvent evt)
        {
            Balance += evt.Amount;
        }

        private void Apply(MoneyWithdrawnEvent evt)
        {
            Balance -= evt.Amount;
        }

        public void RestoreSnapshot(Guid id,string accountHolder,decimal balance,int version)
        {
            Id = id;
            AccountHolder = accountHolder;
            Balance = balance;
            Version = version;

            ClearEvents();
        }

        public void LoadFromHistory(IEnumerable<object> events)
        {
            foreach (var e in events)
            {
                switch (e)
                {
                    case AccountCreatedEvent created:
                        Apply(created);
                        break;

                    case MoneyDepositedEvent deposited:
                        Apply(deposited);
                        break;

                    case MoneyWithdrawnEvent withdrawn:
                        Apply(withdrawn);
                        break;
                }
                Version++;
            }

            ClearEvents();
        }
        public void ClearEvents()
        {
            _uncommittedEvents.Clear();
        }

        public void SetVersion(int v) => Version = v;
    }
}
