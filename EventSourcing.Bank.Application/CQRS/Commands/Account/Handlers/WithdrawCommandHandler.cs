using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.CQRS.Commands;
using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record WithdrawCommand : ICommand<AccountAggregate>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
    public class WithdrawCommandHandler : ICommandHandler<WithdrawCommand, AccountAggregate>
    {
        private readonly IAccountRepository _repository;

        public WithdrawCommandHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountAggregate> HandleAsync(WithdrawCommand command, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(command.AccountId, cancellationToken);
            account.Withdraw(new Money(command.Amount), command.CommandId);
            await _repository.SaveWithSnapshotAsync(account, account.Version, cancellationToken);
            return account;
        }
    }
}
