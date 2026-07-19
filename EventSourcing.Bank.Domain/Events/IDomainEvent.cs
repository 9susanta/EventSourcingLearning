using System;
using MediatR;

namespace EventSourcing.Bank.Domain.Events
{
    public interface IDomainEvent : INotification
    {
        Guid EventId { get; }
        Guid CommandId { get; }
    }
}
