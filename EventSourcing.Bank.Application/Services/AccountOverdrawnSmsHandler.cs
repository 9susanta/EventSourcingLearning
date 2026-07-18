using System;
using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;

namespace EventSourcing.Bank.Application.Services
{
    public class AccountOverdrawnSmsHandler : MediatR.INotificationHandler<AccountOverdrawnEvent>
    {
        public Task Handle(AccountOverdrawnEvent domainEvent, System.Threading.CancellationToken cancellationToken)
        {
            // Simulate sending an SMS
            Console.WriteLine($"[SMS SERVICE] WARNING: Account {domainEvent.AccountId} is overdrawn by {-domainEvent.OverdrawnAmount}!");
            
            // Or log it properly via ILogger
            return Task.CompletedTask;
        }
    }
}
