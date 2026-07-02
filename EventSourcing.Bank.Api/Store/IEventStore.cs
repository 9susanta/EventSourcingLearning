using EventSourcing.Bank.Api.Domain;

namespace EventSourcing.Bank.Api.Store
{
    public interface IEventStore
    {
        void Save(BankAccount account);

        BankAccount Load(Guid accountId);
    }
}
