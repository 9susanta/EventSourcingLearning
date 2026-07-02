using EventSourcing.Bank.Domain.Aggregates;
using EventSourcing.Bank.Application.Abstractions;
using EventSourcing.Bank.Infrastructure.Serialization;
using Microsoft.Data.Sqlite;

namespace EventSourcing.Bank.Infrastructure.Persistence
{
    public class SqliteEventStore : IEventStore
    {
        private readonly string _connectionString;

        public SqliteEventStore(string connectionString)
        {
            _connectionString = connectionString;
            EnsureSchema();
        }

        private void EnsureSchema()
        {
            using var conn = new SqliteConnection(_connectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Events (
                Id TEXT PRIMARY KEY,
                AggregateId TEXT NOT NULL,
                Type TEXT NOT NULL,
                Data TEXT NOT NULL,
                OccurredAt TEXT NOT NULL
            );";
            cmd.ExecuteNonQuery();
        }

        public async Task SaveAsync(AccountAggregate account, int expectedVersion, CancellationToken cancellationToken)
        {
            var events = account.UncommittedEvents;
            if (events == null || !events.Any()) return;

            using var conn = new SqliteConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);

            using (var countCmd = conn.CreateCommand())
            {
                countCmd.CommandText = "SELECT COUNT(1) FROM Events WHERE AggregateId = $agg;";
                countCmd.Parameters.AddWithValue("$agg", account.Id.ToString());
                var current = Convert.ToInt32((long)countCmd.ExecuteScalar());
                if (current != expectedVersion)
                {
                    throw new EventSourcing.Bank.Application.Abstractions.ConcurrencyException("Concurrency conflict: expected version does not match current stream version.");
                }
            }

            using var tran = conn.BeginTransaction();
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tran;

            foreach (var evt in events)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var id = Guid.NewGuid();
                var aggregateId = account.Id;
                var type = evt.GetType().FullName ?? evt.GetType().Name;
                var data = EventSerializer.Serialize(evt);
                var occurredAt = DateTime.UtcNow.ToString("o");

                cmd.CommandText = "INSERT INTO Events (Id, AggregateId, Type, Data, OccurredAt) VALUES ($id, $agg, $type, $data, $at);";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("$id", id.ToString());
                cmd.Parameters.AddWithValue("$agg", aggregateId.ToString());
                cmd.Parameters.AddWithValue("$type", type);
                cmd.Parameters.AddWithValue("$data", data);
                cmd.Parameters.AddWithValue("$at", occurredAt);

                cmd.ExecuteNonQuery();
            }

            tran.Commit();

            account.SetVersion(expectedVersion + events.Count);
            account.ClearEvents();
        }

        public Task<AccountAggregate> LoadAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var events = new List<object>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Type, Data FROM Events WHERE AggregateId = $agg ORDER BY OccurredAt;";
            cmd.Parameters.AddWithValue("$agg", accountId.ToString());

            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                var typeName = rdr.GetString(0);
                var data = rdr.GetString(1);

                var type = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == typeName);

                if (type == null) continue;

                var evt = EventSerializer.Deserialize(data, type);
                if (evt != null) events.Add(evt);
            }

            var account = new AccountAggregate();
            account.LoadFromHistory(events);
            return Task.FromResult(account);
        }

        public Task<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>> GetEventsAsync(Guid accountId, CancellationToken cancellationToken)
        {
            var events = new List<EventSourcing.Bank.Application.DTOs.EventDto>();

            using var conn = new SqliteConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Type, Data, OccurredAt FROM Events WHERE AggregateId = $agg ORDER BY OccurredAt;";
            cmd.Parameters.AddWithValue("$agg", accountId.ToString());

            using var rdr = cmd.ExecuteReader();
            int v = 0;
            while (rdr.Read())
            {
                v++;
                var typeName = rdr.GetString(0);
                var data = rdr.GetString(1);
                var at = DateTime.Parse(rdr.GetString(2));

                events.Add(new EventSourcing.Bank.Application.DTOs.EventDto
                {
                    Type = typeName,
                    Data = data,
                    Version = v,
                    OccurredAt = at
                });
            }

            return Task.FromResult<IEnumerable<EventSourcing.Bank.Application.DTOs.EventDto>>(events);
        }
    }
}
