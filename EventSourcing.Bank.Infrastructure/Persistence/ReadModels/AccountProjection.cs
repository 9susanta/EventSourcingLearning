using System.Threading.Tasks;
using EventSourcing.Bank.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.Bank.Infrastructure.Persistence.ReadModels
{
    public class AccountProjection
    {
        private readonly EventStoreDbContext _db;

        public AccountProjection(EventStoreDbContext db)
        {
            _db = db;
        }

        public async Task ProjectAsync(Guid aggregateId, object @event)
        {
            switch (@event)
            {
                case AccountCreatedEvent e:
                    _db.AccountReadModels.Add(new AccountReadModel
                    {
                        Id = e.AccountId, // or aggregateId
                        AccountHolder = e.AccountHolder,
                        Balance = 0
                    });
                    break;

                case MoneyDepositedEvent e:
                    var accountD = await _db.AccountReadModels.FindAsync(new object[] { aggregateId });
                    if (accountD != null) accountD.Balance += e.Amount.Amount;
                    break;

                case MoneyWithdrawnEvent e:
                    var accountW = await _db.AccountReadModels.FindAsync(new object[] { aggregateId });
                    if (accountW != null) accountW.Balance -= e.Amount.Amount;
                    break;
            }
        }
    }
}
