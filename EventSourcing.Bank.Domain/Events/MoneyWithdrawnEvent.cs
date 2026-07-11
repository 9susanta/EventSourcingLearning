using System;

namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyWithdrawnEvent(decimal Amount, Guid CommandId) : IEvent;
}
