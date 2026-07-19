using EventSourcing.Bank.Domain.Events;
using EventSourcing.Bank.MessageContracts;
using EventSourcing.Bank.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcing.Bank.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Background worker that implements the Outbox Pattern.
    /// It polls the OutboxMessages table and publishes the events to the Message Bus (MassTransit).
    /// </summary>
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OutboxProcessorBackgroundService> _logger;

        public OutboxProcessorBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Outbox Processor is starting...");

            while (!stoppingToken.IsCancellationRequested)
            {
                await ProcessOutboxMessagesAsync(stoppingToken);

                // Wait 3 seconds before checking again (in a real app, this might be shorter or trigger-based)
                await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
            }
        }

        private async Task ProcessOutboxMessagesAsync(CancellationToken stoppingToken)
        {
            // Because BackgroundService is a singleton, we MUST create a scope to resolve scoped dependencies like DbContext.
            using var scope = _serviceProvider.CreateScope();
            
            var dbContext = scope.ServiceProvider.GetRequiredService<EventStoreDbContext>();
            var publisher = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            // Get top 20 unprocessed messages that are not in the Dead Letter Queue
            var messages = await dbContext.OutboxMessages
                .Where(m => m.ProcessedDate == null && m.DeadLetterDate == null)
                .OrderBy(m => m.OccurredOn)
                .Take(20)
                .ToListAsync(stoppingToken);

            if (!messages.Any())
                return;

            foreach (var message in messages)
            {
                try
                {
                    // 1. Deserialize the stored JSON payload back into the strongly typed Domain Event
                    var eventType = Type.GetType(message.Type);
                    if (eventType == null)
                    {
                        _logger.LogError("Could not resolve type {Type}", message.Type);
                        message.Error = "Type not found";
                        continue;
                    }

                    var domainEvent = JsonSerializer.Deserialize(message.Content, eventType) as EventSourcing.Bank.Domain.Events.IDomainEvent;
                    if (domainEvent == null)
                    {
                        _logger.LogError("Could not deserialize message {Id}", message.Id);
                        message.Error = "Deserialization failed";
                        continue;
                    }

                    // 2. Map to Integration Event and Publish (MassTransit)
                    object? integrationEvent = domainEvent switch
                    {
                        EventSourcing.Bank.Domain.Events.MoneyDepositedEvent d => new EventSourcing.Bank.MessageContracts.MoneyDepositedEvent(d.EventId, d.Amount.Amount),
                        EventSourcing.Bank.Domain.Events.MoneyWithdrawnEvent w => new EventSourcing.Bank.MessageContracts.MoneyWithdrawnEvent(w.EventId, w.Amount.Amount),
                        _ => null
                    };

                    if (integrationEvent != null)
                    {
                        _logger.LogInformation("Publishing Integration Event for Outbox Message {Id}", message.Id);
                        await publisher.Publish(integrationEvent, integrationEvent.GetType(), stoppingToken);
                    }
                    else
                    {
                        _logger.LogInformation("No Integration Event mapping found for {Type}. Skipping publish.", eventType.Name);
                    }

                    // 3. Mark as processed so it doesn't get picked up again
                    message.ProcessedDate = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing outbox message {Id}", message.Id);
                    message.Error = ex.Message;
                    message.RetryCount++;

                    // POISON MESSAGE HANDLING (DLQ)
                    if (message.RetryCount >= 3)
                    {
                        _logger.LogWarning("Message {Id} has failed {Count} times. Moving to Dead Letter Queue.", message.Id, message.RetryCount);
                        message.DeadLetterDate = DateTime.UtcNow;
                    }
                }
            }

            // Save the updated ProcessedDates back to SQL Server
            await dbContext.SaveChangesAsync(stoppingToken);
        }
    }
}
