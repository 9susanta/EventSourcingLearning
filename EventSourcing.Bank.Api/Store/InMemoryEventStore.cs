using EventSourcing.Bank.Api.Domain;

namespace EventSourcing.Bank.Api.Store
{
    public class InMemoryEventStore :IEventStore
    {
        private readonly Dictionary<Guid, List<object>> _store = new();

        // Called by application/service to persist an aggregate's uncommitted events and clear them
        public void Save(BankAccount account)
        {
            if (!_store.ContainsKey(account.Id))
            {
                _store[account.Id] = new List<object>();
            }

            _store[account.Id].AddRange(account.UncommittedEvents);

            account.ClearEvents();
        }

        // Called by Load to fetch the historical event stream for an aggregate
        public List<object> GetEvents(Guid accountId)
        {
            if (_store.TryGetValue(accountId, out var events))
            {
                return events;
            }

            return new List<object>();
        }

        // Called by repository/service to rebuild an aggregate by replaying its events
        public BankAccount Load(Guid accountId)
        {
            var account = new BankAccount();

            var events = GetEvents(accountId);

            account.LoadFromHistory(events);

            return account;
        }
    }
}
