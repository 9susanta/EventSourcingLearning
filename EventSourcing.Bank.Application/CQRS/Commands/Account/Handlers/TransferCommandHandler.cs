using System;
using System.Threading;
using System.Threading.Tasks;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.CQRS.Commands;
using EventSourcing.Bank.Domain.Services;
using EventSourcing.Bank.Domain.ValueObjects;

namespace EventSourcing.Bank.Application.CQRS.Commands.Account.Handlers
{
    public record TransferCommand : MediatR.IRequest<bool>
    {
        public Guid SourceAccountId { get; set; }
        public Guid DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
        public Guid CommandId { get; set; } = Guid.NewGuid();
    }

    public class TransferCommandHandler : MediatR.IRequestHandler<TransferCommand, bool>
    {
        private readonly IAccountRepository _repository;
        private readonly FundsTransferDomainService _domainService;

        public TransferCommandHandler(IAccountRepository repository, FundsTransferDomainService domainService)
        {
            _repository = repository;
            _domainService = domainService;
        }

        public async Task<bool> Handle(TransferCommand command, CancellationToken cancellationToken)
        {
            var source = await _repository.GetByIdAsync(command.SourceAccountId, cancellationToken);
            var destination = await _repository.GetByIdAsync(command.DestinationAccountId, cancellationToken);
            
            var money = new Money(command.Amount);

            _domainService.Transfer(source, destination, money, command.CommandId);

            // Unit of work conceptually saves both. 
            // Since this project uses an EventStore which saves per stream, we'll save them individually.
            await _repository.SaveAsync(source, source.Version, cancellationToken);
            await _repository.SaveAsync(destination, destination.Version, cancellationToken);

            return true;
        }
    }
}
