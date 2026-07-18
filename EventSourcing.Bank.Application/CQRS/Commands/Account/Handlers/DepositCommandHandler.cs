using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record DepositCommand : MediatR.IRequest<AccountAggregate>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
    public class DepositCommandHandler : MediatR.IRequestHandler<DepositCommand, AccountAggregate>
    {
        private readonly IAccountRepository _repository;

        public DepositCommandHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountAggregate> Handle(DepositCommand command, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(command.AccountId, cancellationToken);
            account.Deposit(new Money(command.Amount), command.CommandId);
            await _repository.SaveWithSnapshotAsync(account, account.Version, cancellationToken);
            return account;
        }
    }
}
