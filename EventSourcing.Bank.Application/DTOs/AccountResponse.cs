using System;

namespace EventSourcing.Bank.Application.DTOs
{
    public class AccountResponse
    {
        public Guid Id { get; set; }
        public string AccountHolder { get; set; }
        public decimal Balance { get; set; }
    }
}
