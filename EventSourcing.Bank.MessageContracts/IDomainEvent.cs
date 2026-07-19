using System;

namespace EventSourcing.Bank.MessageContracts
{
    public interface IDomainEvent
    {
        /// <summary>
        /// A globally unique identifier for this specific event occurrence.
        /// This is crucial for Idempotency checks (Inbox Pattern).
        /// </summary>
        Guid EventId { get; }
    }
}
