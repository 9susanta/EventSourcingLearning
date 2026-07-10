using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;

namespace EventSourcing.Bank.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountAggregate> CreateAccountAsync(string accountHolder, CancellationToken cancellationToken)
        {
            var account = AccountAggregate.Create(accountHolder);

            await _repository.SaveAsync(account, 0, cancellationToken);

            return account;
        }

        public async Task<AccountAggregate> DepositAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(accountId, cancellationToken);

            account.Deposit(amount);

            await _repository.SaveWithSnapshotAsync(account, account.Version, cancellationToken);

            return account;
        }

        public async Task<AccountAggregate> WithdrawAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(accountId, cancellationToken);

            account.Withdraw(amount);

            await _repository.SaveWithSnapshotAsync(account, account.Version, cancellationToken);

            return account;
        }

        public Task<AccountAggregate> GetAsync(Guid accountId, CancellationToken cancellationToken)
        { 
            return _repository.GetByIdAsync(accountId, cancellationToken);
        }
    }
}
