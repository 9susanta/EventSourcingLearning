using System;

namespace EventSourcing.Bank.Domain.Events
{
    public record AccountCreatedEvent(Guid AccountId, string AccountHolder, Guid CommandId) : IDomainEvent
    {
        public Guid EventId { get; } = Guid.NewGuid();
    }
}
