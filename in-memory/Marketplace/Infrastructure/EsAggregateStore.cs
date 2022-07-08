using System.Text;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure;

public class EsAggregateStore : IAggregateStore
{
  private readonly IEventStoreConnection _connection;

  public EsAggregateStore(IEventStoreConnection connection)
    => _connection = connection;

  public async Task<bool> Exists<T, TId>(TId aggregateId)
  {
    string stream = GetStreamName<T, TId>(aggregateId);

    EventReadResult result = await _connection.ReadEventAsync(stream,
      eventNumber: 1, resolveLinkTos: false);

    return result.Status != EventReadStatus.NoStream;
  }

  public async Task<T> Load<T, TId>(TId aggregateId)
    where T : AggregateRoot<TId>
    where TId : ValueObject
  {
    // 1. Ensures that the aggregate ID parameter is not null.
    if (aggregateId is null)
    {
      throw new ArgumentNullException(nameof(aggregateId));
    }

    // 2. Gets the stream name for a given aggregate type.
    string stream = GetStreamName<T, TId>(aggregateId);

    // 3. Creates a new instance of the aggregate type by using reflections.
    var aggregate = (T)Activator.CreateInstance(type: typeof(T),
      nonPublic: true)!;

    // 4. Reads events from the stream as a collection of ResolvedEvent objects.
    StreamEventsSlice page = await _connection.ReadStreamEventsForwardAsync(
      stream,
      start: 0,
      count: 1024,
      resolveLinkTos: false);

    // 5. Calls the Load method of empty aggregate instance to recover the
    // aggregate state.
    // 6. Deserializes those raw events to a collection of domain events.
    aggregate.Load(page.Events.Select(
      resolvedEvent => resolvedEvent.Deserialize()).ToArray());

    return aggregate;
  }

  public async Task Save<T, TId>(T aggregate)
    where T : AggregateRoot<TId>
    where TId : ValueObject
  {
    if (aggregate is null)
    {
      throw new ArgumentNullException(nameof(aggregate));
    }

    EventData[] changes = aggregate.GetChanges()
      .Select(@event =>
        new EventData(
          eventId: Guid.NewGuid(),
          type: @event.GetType().Name,
          isJson: true,
          data: Serialize(@event),
          metadata: Serialize(new EventMetadata
          {
            ClrType = @event.GetType().AssemblyQualifiedName!
          })
        )
      )
      .ToArray();

    if (changes.Length == 0)
    {
      return;
    }

    string streamName = GetStreamName<T, TId>(aggregate);

    _ = await _connection.AppendToStreamAsync(
      streamName,
      aggregate.Version,
      changes
    );

    aggregate.ClearChanges();
  }

  private static string GetStreamName<T, TId>(TId aggregateId)
    => $"{typeof(T).Name}-{aggregateId}";

  private static string GetStreamName<T, TId>(T aggregate)
    where T : AggregateRoot<TId>
    where TId : ValueObject
      => $"{typeof(T).Name}-{aggregate.Id}";

  private static byte[] Serialize(object data)
    => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
}