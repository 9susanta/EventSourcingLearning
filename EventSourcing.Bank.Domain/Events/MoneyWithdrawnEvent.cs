using System;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyWithdrawnEvent(Money Amount, Guid CommandId);
}
