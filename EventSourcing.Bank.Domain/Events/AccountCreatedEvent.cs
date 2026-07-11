using System;

namespace EventSourcing.Bank.Domain.Events
{
    public record AccountCreatedEvent(Guid AccountId, string AccountHolder):IEvent;
}
