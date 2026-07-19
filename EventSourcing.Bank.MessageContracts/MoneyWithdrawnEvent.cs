using System;

namespace EventSourcing.Bank.MessageContracts
{
    public record MoneyWithdrawnEvent(Guid AccountId, decimal Amount) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
