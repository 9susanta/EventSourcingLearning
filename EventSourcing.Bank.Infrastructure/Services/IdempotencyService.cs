using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bank.Infrastructure.Services
{
    public class IdempotencyService : IIdempotencyService
    {
        private readonly EventStoreDbContext _db;

        public IdempotencyService(EventStoreDbContext db)
        {
            _db = db;
        }

        public async Task<bool> HasBeenProcessedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            // Inbox Check
            return await _db.InboxMessages.AnyAsync(m => m.Id == eventId, cancellationToken);
        }

        public async Task MarkAsProcessedAsync(Guid eventId, CancellationToken cancellationToken)
        {
            var inboxMessage = new InboxMessage
            {
                Id = eventId,
                ProcessedAt = DateTime.UtcNow
            };

            _db.InboxMessages.Add(inboxMessage);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
