namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyDepositedEvent(decimal Amount):IEvent;
}
