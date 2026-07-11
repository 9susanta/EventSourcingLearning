using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.CQRS.Commands;
using EventSourcing.Bank.Application.Services;
using EventSourcing.Bank.Domain.Aggregates;

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
        private readonly IAccountService _accountService;
        public WithdrawCommandHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<AccountAggregate> HandleAsync(WithdrawCommand command, CancellationToken cancellationToken)
        {
            return await _accountService.WithdrawAsync(command.AccountId, command.Amount, command.CommandId, cancellationToken);
        }
    }
}
