namespace EventSourcing.Bank.Api.Events
{
    public record AccountCreated(Guid AccountId,string AccountHolder);
}
