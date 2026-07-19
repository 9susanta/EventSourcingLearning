using System;

namespace EventSourcing.Bank.MessageContracts
{
    public record AccountOverdrawnEvent(Guid AccountId, decimal OverdrawnAmount) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
