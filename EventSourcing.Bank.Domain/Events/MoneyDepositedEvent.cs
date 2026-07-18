using System;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyDepositedEvent(Money Amount, Guid CommandId) : IDomainEvent;
}
