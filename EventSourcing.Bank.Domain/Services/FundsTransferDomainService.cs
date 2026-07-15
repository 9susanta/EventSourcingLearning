using System;
using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Domain.Services
{
    public class FundsTransferDomainService
    {
        public void Transfer(AccountAggregate source, AccountAggregate destination, Money amount, Guid commandId)
        {
            if (source.Balance.Currency != destination.Balance.Currency)
                throw new InvalidOperationException("Cannot transfer between different currencies.");

            // Withdraw from source and Deposit to destination
            // In a real system, you'd use separate command IDs or handle idempotency slightly differently for the dual actions.
            source.Withdraw(amount, commandId);
            destination.Deposit(amount, commandId);
        }
    }
}
