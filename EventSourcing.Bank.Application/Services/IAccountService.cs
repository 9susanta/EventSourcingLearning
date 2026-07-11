using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Application.Services
{
    public interface IAccountService
    {
        Task<AccountAggregate> CreateAccountAsync(string accountHolder, Guid commandId, CancellationToken cancellationToken);
        Task<AccountAggregate> DepositAsync(Guid accountId, decimal amount, Guid commandId, CancellationToken cancellationToken);
        Task<AccountAggregate> WithdrawAsync(Guid accountId, decimal amount, Guid commandId, CancellationToken cancellationToken);
        Task<AccountAggregate> GetAsync(Guid accountId, CancellationToken cancellationToken);
    }
}
