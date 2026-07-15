using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record CreateAccountCommand : ICommand<AccountAggregate>
    {
        public string AccountHolder { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, AccountAggregate>
    {
        private readonly IAccountRepository _repository;

        public CreateAccountCommandHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountAggregate> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            var account = AccountAggregate.Create(command.AccountHolder, command.CommandId);
            await _repository.SaveAsync(account, 0, cancellationToken);
            return account;
        }
    }
}
