using EventSourcing.Bank.Application.CQRS.Queries;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Application.DTOs;

namespace EventSourcing.Bank.Application.CQRS.Queries.Account.Handlers
{
    public record GetAccountHistoryQuery : MediatR.IRequest<IEnumerable<EventDto>>
    {
        public Guid AccountId { get; set; }

    }
    public class GetAccountHistoryQueryHandler : MediatR.IRequestHandler<GetAccountHistoryQuery, IEnumerable<EventDto>>
    {
        private readonly IEventStore _eventStore;

        public GetAccountHistoryQueryHandler(IEventStore eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<IEnumerable<EventDto>> Handle(GetAccountHistoryQuery query, CancellationToken cancellationToken)
        {
            return await _eventStore.GetEventsAsync(query.AccountId, cancellationToken);
        }
    }
}
