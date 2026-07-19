using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Modules.Email
{
    public interface IEmailIdempotencyService
    {
        Task<bool> HasBeenProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
        Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    }

    public class EmailIdempotencyService : IEmailIdempotencyService
    {
        private readonly EmailDbContext _dbContext;

        public EmailIdempotencyService(EmailDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> HasBeenProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.InboxMessages.AnyAsync(m => m.Id == messageId, cancellationToken);
        }

        public async Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default)
        {
            _dbContext.InboxMessages.Add(new InboxMessage
            {
                Id = messageId,
                ProcessedDate = DateTime.UtcNow
            });
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
