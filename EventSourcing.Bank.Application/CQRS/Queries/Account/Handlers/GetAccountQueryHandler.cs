using EventSourcing.Bank.Application.CQRS.Queries;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.DTOs;

namespace EventSourcing.Bank.Application.CQRS.Queries.Account.Handlers
{
    public record GetAccountQuery : MediatR.IRequest<AccountResponse>
    {
        public Guid AccountId { get; set; }
    }
    public class GetAccountQueryHandler : MediatR.IRequestHandler<GetAccountQuery, AccountResponse>
    {
        private readonly IAccountRepository _repository;

        public GetAccountQueryHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountResponse> Handle(GetAccountQuery query, CancellationToken cancellationToken)
        {
            // Now reading from the highly-optimized Read Model instead of replaying events!
            return await _repository.GetReadModelAsync(query.AccountId, cancellationToken);
        }
    }
}
