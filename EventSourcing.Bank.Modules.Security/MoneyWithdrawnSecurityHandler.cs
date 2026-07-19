using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bank.MessageContracts;
using EventSourcing.Bank.MessageContracts;
using MassTransit;
using Polly;

namespace EventSourcing.Bank.Modules.Security
{
    public class MoneyWithdrawnSecurityHandler : IConsumer<MoneyWithdrawnEvent>
    {
        private readonly ISecurityIdempotencyService _idempotencyService;

        public MoneyWithdrawnSecurityHandler(ISecurityIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }

        public async Task Consume(ConsumeContext<MoneyWithdrawnEvent> context)
        {
            var domainEvent = context.Message;
            // 1. INBOX PATTERN: Idempotency Check
            if (await _idempotencyService.HasBeenProcessedAsync(domainEvent.EventId, context.CancellationToken))
            {
                Console.WriteLine($"[SECURITY SERVICE] IGNORING: Message {domainEvent.EventId} has already been processed.");
                return;
            }

            // 2. RETRY PATTERN (Polly): Exponential Backoff
            var retryPolicy = Policy
                .Handle<Exception>() 
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, context) => 
                {
                    Console.WriteLine($"[SECURITY SERVICE] FAILED. Retrying in {timeSpan.TotalSeconds}s... Error: {exception.Message}");
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                // Execute actual business logic
                Console.WriteLine($"[SECURITY SERVICE] INFO: A withdrawal of {domainEvent.Amount} is being monitored for fraud.");
                await Task.CompletedTask; 
            });

            // 3. INBOX PATTERN: Save Processed State
            await _idempotencyService.MarkAsProcessedAsync(domainEvent.EventId, context.CancellationToken);
        }
    }
}
