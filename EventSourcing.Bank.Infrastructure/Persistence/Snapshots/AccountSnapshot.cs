using System;
using System.Collections.Generic;
using System.Text;

namespace EventSourcing.Bank.Infrastructure.Persistence.Snapshots
{
    public class AccountSnapshot
    {
        public Guid Id { get; set; }

        public string AccountHolder { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public int Version { get; set; }
    }
}
