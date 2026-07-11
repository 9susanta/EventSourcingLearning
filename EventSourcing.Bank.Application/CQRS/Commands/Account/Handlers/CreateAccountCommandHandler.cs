using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Services;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record CreateAccountCommand : ICommand<AccountAggregate>
    {
        public string AccountHolder { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
    public class CreateAccountCommandHandler : ICommandHandler<CreateAccountCommand, AccountAggregate>
    {
        private readonly IAccountService _accountService;

        public CreateAccountCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<AccountAggregate> HandleAsync(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            // Note: CommandId is passed down to the service to ensure idempotency
            return await _accountService.CreateAccountAsync(command.AccountHolder, command.CommandId, cancellationToken);
        }
    }
}
