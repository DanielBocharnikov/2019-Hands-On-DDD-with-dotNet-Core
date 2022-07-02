using EventStore.ClientAPI;

namespace Marketplace;

public class EventStoreConnectionService : BackgroundService
{
  private readonly IEventStoreConnection _esConnection;

  public EventStoreConnectionService(IEventStoreConnection esConnection)
    => _esConnection = esConnection;

  protected override Task ExecuteAsync(CancellationToken stoppingToken)
    => _esConnection.ConnectAsync();

  public override void Dispose()
  {
    _esConnection.Close();
    GC.SuppressFinalize(this);
    base.Dispose();
  }
}