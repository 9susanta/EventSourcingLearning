using System;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Domain.Events
{
    public record AccountOverdrawnEvent(Guid AccountId, Money Amount, Guid CommandId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
