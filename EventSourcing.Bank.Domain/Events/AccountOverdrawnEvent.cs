using System;

namespace EventSourcing.Bank.Domain.Events
{
    public interface IDomainEvent : MediatR.INotification
    {
    }

    public record AccountOverdrawnEvent(Guid AccountId, decimal OverdrawnAmount) : IDomainEvent;
}
