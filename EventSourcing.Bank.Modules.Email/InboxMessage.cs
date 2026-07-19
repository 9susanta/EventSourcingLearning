using System;

namespace EventSourcing.Bank.Modules.Email
{
    public class InboxMessage
    {
        public Guid Id { get; set; }
        public DateTime ProcessedDate { get; set; }
    }
}
