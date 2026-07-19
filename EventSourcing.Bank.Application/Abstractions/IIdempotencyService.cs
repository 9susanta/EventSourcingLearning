using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bank.Application.Abstractions
{
    /// <summary>
    /// Service to enforce Idempotency for Message Consumers.
    /// Uses the Inbox Pattern behind the scenes.
    /// </summary>
    public interface IIdempotencyService
    {
        /// <summary>
        /// Checks if an event has already been processed.
        /// </summary>
        Task<bool> HasBeenProcessedAsync(Guid eventId, CancellationToken cancellationToken);

        /// <summary>
        /// Marks an event as processed to prevent future duplicate processing.
        /// </summary>
        Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken);
    }
}
