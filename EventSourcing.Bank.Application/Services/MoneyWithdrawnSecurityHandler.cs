using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;
using MediatR;

namespace EventSourcing.Bank.Application.Services
{
    public class MoneyWithdrawnSecurityHandler : INotificationHandler<MoneyWithdrawnEvent>
    {
        public Task Handle(MoneyWithdrawnEvent domainEvent, CancellationToken cancellationToken)
        {
            // Simulate triggering a fraud check or security notification
            Console.WriteLine($"[SECURITY SERVICE] INFO: A withdrawal of {domainEvent.Amount.Amount} is being monitored for fraud.");
            
            return Task.CompletedTask;
        }
    }
}
