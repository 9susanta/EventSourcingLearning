using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Services;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record CreateAccountCommand : ICommand<AccountAggregate>
    {
        public string AccountHolder { get; set; }
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
            return await _accountService.CreateAccountAsync(command.AccountHolder, cancellationToken);
        }
    }
}
