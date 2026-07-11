using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Domain.Aggregates;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record DepositCommand : ICommand<AccountAggregate>
    {
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }
    public class DepositCommandHandler : ICommandHandler<DepositCommand, AccountAggregate>
    {
        private readonly IAccountService _accountService;

        public DepositCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<AccountAggregate> HandleAsync(DepositCommand command, CancellationToken cancellationToken)
        {
            return await _accountService.DepositAsync(command.AccountId, command.Amount, command.CommandId, cancellationToken);
        }
    }
}
