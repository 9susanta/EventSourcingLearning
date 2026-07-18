using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;
using MediatR;

namespace EventSourcing.Bank.Application.Services
{
    public class MoneyDepositedEmailHandler : INotificationHandler<MoneyDepositedEvent>
    {
        public Task Handle(MoneyDepositedEvent domainEvent, CancellationToken cancellationToken)
        {
            // Simulate sending a confirmation email to the user
            Console.WriteLine($"[EMAIL SERVICE] SUCCESS: A deposit of {domainEvent.Amount.Amount} has been processed.");
            
            return Task.CompletedTask;
        }
    }
}
