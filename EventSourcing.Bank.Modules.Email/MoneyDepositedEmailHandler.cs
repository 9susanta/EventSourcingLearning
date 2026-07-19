using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bank.MessageContracts;
using EventSourcing.Bank.MessageContracts;
using MassTransit;
using Polly;

namespace EventSourcing.Bank.Modules.Email
{
    public class MoneyDepositedEmailHandler : IConsumer<MoneyDepositedEvent>
    {
        private readonly IEmailIdempotencyService _idempotencyService;

        public MoneyDepositedEmailHandler(IEmailIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }

        public async Task Consume(ConsumeContext<MoneyDepositedEvent> context)
        {
            var domainEvent = context.Message;
            // 1. INBOX PATTERN: Idempotency Check
            // If Azure Service Bus delivers this message twice (due to network failure), we won't send the email twice!
            if (await _idempotencyService.HasBeenProcessedAsync(domainEvent.EventId, context.CancellationToken))
            {
                Console.WriteLine($"[EMAIL SERVICE] IGNORING: Message {domainEvent.EventId} has already been processed.");
                return;
            }

            // 2. RETRY PATTERN (Polly): Exponential Backoff
            // We simulate a network failure. If sending the email fails, we try 3 times, waiting 2s, 4s, 8s.
            var retryPolicy = Policy
                .Handle<Exception>() // In production, ONLY handle transient exceptions (e.g., HttpRequestException)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, context) => 
                {
                    Console.WriteLine($"[EMAIL SERVICE] FAILED. Retrying in {timeSpan.TotalSeconds}s... Error: {exception.Message}");
                });

            await retryPolicy.ExecuteAsync(async () =>
            {
                // Execute actual business logic
                Console.WriteLine($"[EMAIL SERVICE] SUCCESS: A deposit of {domainEvent.Amount} has been processed.");
                await Task.CompletedTask; // Simulate async work
            });

            // 3. INBOX PATTERN: Save Processed State
            // Only after successful execution do we record that we processed this message.
            await _idempotencyService.MarkAsProcessedAsync(domainEvent.EventId, context.CancellationToken);
        }
    }
}
