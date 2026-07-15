using System;

namespace EventSourcing.Bank.Domain.Events
{
    public interface IDomainEvent
    {
    }

    public record AccountOverdrawnEvent(Guid AccountId, decimal OverdrawnAmount) : IDomainEvent;
}
