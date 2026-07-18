using System;

namespace EventSourcing.Bank.Application.DTOs
{
    public class TransferRequest
    {
        public Guid DestinationAccountId { get; set; }
        public decimal Amount { get; set; }
    }
}
