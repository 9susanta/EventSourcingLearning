using System;

namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyDepositedEvent(decimal Amount, Guid CommandId) : IEvent;
}
