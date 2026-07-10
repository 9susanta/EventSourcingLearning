using EventSourcing.Bank.Application.CQRS.Queries;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.DTOs;

namespace EventSourcing.Bank.Application.CQRS.Queries.Account.Handlers
{
    public record GetAccountQuery : IQuery<AccountResponse>
    {
        public Guid AccountId { get; set; }
    }
    public class GetAccountQueryHandler : IQueryHandler<GetAccountQuery, AccountResponse>
    {
        private readonly IAccountRepository _repository;

        public GetAccountQueryHandler(IAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<AccountResponse> HandleAsync(GetAccountQuery query, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByIdAsync(query.AccountId, cancellationToken);
            return new AccountResponse
            {
                Id = account.Id,
                AccountHolder = account.AccountHolder,
                Balance = account.Balance
            };
        }
    }
}
