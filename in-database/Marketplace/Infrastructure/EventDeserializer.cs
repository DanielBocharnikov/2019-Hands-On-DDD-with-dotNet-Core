using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure;

public static class EventDeserializer
{
  public static object Deserialize(this ResolvedEvent resolvedEvent)
  {
    EventMetadata meta = JsonConvert.DeserializeObject<EventMetadata>(
      Encoding.UTF8.GetString(resolvedEvent.Event.Metadata)
    )!;

    Type dataType = Type.GetType(meta.ClrType)!;

    string jsonData = Encoding.UTF8.GetString(resolvedEvent.Event.Data);

    return JsonConvert.DeserializeObject(jsonData, dataType)!;
  }
}