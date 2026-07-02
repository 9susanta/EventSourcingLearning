namespace EventSourcing.Bank.Infrastructure.Persistence
{
    public class EventEntity
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public int Version { get; set; }
    }
}
