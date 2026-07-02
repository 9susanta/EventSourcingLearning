namespace EventSourcing.Bank.Infrastructure.Serialization
{
    public static class EventSerializer
    {
        public static string Serialize(object evt) => System.Text.Json.JsonSerializer.Serialize(evt);

        public static object? Deserialize(string json, System.Type type) => System.Text.Json.JsonSerializer.Deserialize(json, type);
    }
}
