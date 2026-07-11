using System;
using System.ComponentModel.DataAnnotations;

namespace EventSourcing.Bank.Infrastructure.Persistence.ReadModels
{
    public class AccountReadModel
    {
        [Key]
        public Guid Id { get; set; }
        public string AccountHolder { get; set; }
        public decimal Balance { get; set; }
    }
}
