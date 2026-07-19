using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Modules.Security
{
    public interface ISecurityIdempotencyService
    {
        Task<bool> HasBeenProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
        Task MarkAsProcessedAsync(Guid messageId, CancellationToken cancellationToken = default);
    }

    public class SecurityIdempotencyService : ISecurityIdempotencyService
    {
        private readonly SecurityDbContext _dbContext;

        public SecurityIdempotencyService(SecurityDbContext dbContext)
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
