using System;
using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;

namespace EventSourcing.Bank.Application.Services
{
    public class AccountOverdrawnSmsHandler : IDomainEventHandler<AccountOverdrawnEvent>
    {
        public Task HandleAsync(AccountOverdrawnEvent domainEvent)
        {
            // Simulate sending an SMS
            Console.WriteLine($"[SMS SERVICE] WARNING: Account {domainEvent.AccountId} is overdrawn by {-domainEvent.OverdrawnAmount}!");
            
            // Or log it properly via ILogger
            return Task.CompletedTask;
        }
    }
}
