using System;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    /// <summary>
    /// Used by our Consumers (Event Handlers) to record which messages they have already processed.
    /// This guarantees Idempotency (we will never process the same message twice, even if Service Bus redelivers it).
    /// </summary>
    public class InboxMessage
    {
        /// <summary>
        /// The unique ID of the Domain Event.
        /// </summary>
        public Guid Id { get; set; }
        
        public DateTime ProcessedAt { get; set; }
    }
}
