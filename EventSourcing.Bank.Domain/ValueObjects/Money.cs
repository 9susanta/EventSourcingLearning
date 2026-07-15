using System;

namespace EventSourcing.Bank.Domain.ValueObjects
{
    public record Money
    {
        public decimal Amount { get; init; }
        public string Currency { get; init; }

        // Parameterless constructor needed for deserialization/EF Core
        public Money() { }

        public Money(decimal amount, string currency = "USD")
        {
            Amount = amount;
            Currency = currency;
        }

        public Money Add(Money other)
        {
            if (this.Currency != other.Currency)
                throw new InvalidOperationException("Currency mismatch!");
            
            return new Money(this.Amount + other.Amount, this.Currency);
        }

        public Money Subtract(Money other)
        {
            if (this.Currency != other.Currency)
                throw new InvalidOperationException("Currency mismatch!");

            return new Money(this.Amount - other.Amount, this.Currency);
        }
    }
}
