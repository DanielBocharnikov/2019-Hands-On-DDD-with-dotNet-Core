using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure;

public static class EventStoreExtensions
{
  public static Task AppendEvents(this IEventStoreConnection connection,
    string streamName, long version, params object[] events)
  {
    if (events?.Any() != true)
    {
      return Task.CompletedTask;
    }

    EventData[] preparedEvents = events
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
        ))
      .ToArray();

    if (preparedEvents is null || preparedEvents.Length == 0)
    {
      return Task.CompletedTask;
    }

    return connection.AppendToStreamAsync(streamName, version, preparedEvents);
  }

  private static byte[] Serialize(object data)
  {
    string jsonData = JsonConvert.SerializeObject(data);

    return Encoding.UTF8.GetBytes(jsonData);
  }
}