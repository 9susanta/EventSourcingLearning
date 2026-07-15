using System;
using System.Collections.Generic;
using System.Linq;
using EventSourcing.Bank.Domain.Events;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Domain.Aggregates
{
    public class AccountAggregate
    {
        public int Version { get; private set; }
        public Guid Id { get; private set; }

        public string AccountHolder { get; private set; }

        public Money Balance { get; private set; }

        private readonly List<object> _uncommittedEvents = new();
        private readonly HashSet<Guid> _processedCommands = new();

        public IReadOnlyCollection<object> UncommittedEvents => _uncommittedEvents.AsReadOnly();

        private readonly List<object> _domainEvents = new();
        public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();

        public void ClearDomainEvents() => _domainEvents.Clear();

        public bool HasProcessedCommand(Guid commandId) => _processedCommands.Contains(commandId);

        public static AccountAggregate Create(string accountHolder, Guid commandId)
        {
            var account = new AccountAggregate();

            var evt = new AccountCreatedEvent(
                Guid.NewGuid(),
                accountHolder,
                commandId);

            account.Apply(evt);

            account._uncommittedEvents.Add(evt);

            return account;
        }

        public void Deposit(Money amount, Guid commandId)
        {
            if (HasProcessedCommand(commandId)) return;

            var evt = new MoneyDepositedEvent(amount, commandId);

            Apply(evt);

            _uncommittedEvents.Add(evt);
        }

        public void Withdraw(Money amount, Guid commandId)
        {
            if (HasProcessedCommand(commandId)) return;

            var evt = new MoneyWithdrawnEvent(amount, commandId);

            Apply(evt);

            if (Balance.Amount < 0)
            {
                _domainEvents.Add(new AccountOverdrawnEvent(Id, Balance.Amount));
            }

            _uncommittedEvents.Add(evt);
        }

        private void Apply(AccountCreatedEvent evt)
        {
            Id = evt.AccountId;
            AccountHolder = evt.AccountHolder;
            Balance = new Money(0);
            _processedCommands.Add(evt.CommandId);
        }

        private void Apply(MoneyDepositedEvent evt)
        {
            Balance = Balance != null ? Balance.Add(evt.Amount) : evt.Amount;
            _processedCommands.Add(evt.CommandId);
        }

        private void Apply(MoneyWithdrawnEvent evt)
        {
            Balance = Balance != null ? Balance.Subtract(evt.Amount) : new Money(-evt.Amount.Amount);
            _processedCommands.Add(evt.CommandId);
        }

        public void RestoreSnapshot(Guid id,string accountHolder,decimal balance,int version)
        {
            Id = id;
            AccountHolder = accountHolder;
            Balance = new Money(balance);
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
