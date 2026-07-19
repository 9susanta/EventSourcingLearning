using System;

namespace EventSourcing.Bank.Modules.Security
{
    public class InboxMessage
    {
        public Guid Id { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
