using System;

namespace EventSourcing.Bank.Application.DTOs
{
    public class EventDto
    {
        public string Type { get; set; }
        public string Data { get; set; }
        public int Version { get; set; }
        public DateTime OccurredAt { get; set; }
    }
}
