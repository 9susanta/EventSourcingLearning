using System;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    /// <summary>
    /// Represents a message (Domain Event) that needs to be published to a Message Broker (like Azure Service Bus).
    /// This is saved in the SAME transaction as our business entities to guarantee no messages are lost if the app crashes.
    /// </summary>
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        
        /// <summary>
        /// The fully qualified type name of the Domain Event so we can deserialize it later.
        /// </summary>
        public string Type { get; set; } = string.Empty;
        
        /// <summary>
        /// The JSON serialized payload of the Domain Event.
        /// </summary>
        public string Content { get; set; } = string.Empty;
        
        public DateTime OccurredOn { get; set; }
        
        /// <summary>
        /// If this is null, the message has NOT been sent to the message broker yet.
        /// </summary>
        public DateTime? ProcessedDate { get; set; }
        
        public string? Error { get; set; }

        /// <summary>
        /// Tracks how many times the background processor has attempted to process this message.
        /// </summary>
        public int RetryCount { get; set; } = 0;

        /// <summary>
        /// If the message fails too many times, it is moved to the Dead Letter Queue by setting this date.
        /// </summary>
        public DateTime? DeadLetterDate { get; set; }
    }
}
