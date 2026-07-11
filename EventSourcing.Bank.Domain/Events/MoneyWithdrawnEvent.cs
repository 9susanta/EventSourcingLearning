namespace EventSourcing.Bank.Domain.Events
{
    public record MoneyWithdrawnEvent(decimal Amount): IEvent;
}
