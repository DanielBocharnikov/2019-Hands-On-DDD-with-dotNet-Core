using EventStore.ClientAPI;

namespace Marketplace.Infrastructure;

public class Checkpoint
{
  public string Id { get; set; } = string.Empty;

  public Position Position { get; set; }
}