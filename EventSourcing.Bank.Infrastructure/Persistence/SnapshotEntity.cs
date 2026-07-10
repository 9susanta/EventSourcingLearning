using System;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    public class SnapshotEntity
    {
        public Guid Id { get; set; }
        public Guid AggregateId { get; set; }
        public int Version { get; set; }
        public string AggregateType { get; set; }
        public string Data { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
