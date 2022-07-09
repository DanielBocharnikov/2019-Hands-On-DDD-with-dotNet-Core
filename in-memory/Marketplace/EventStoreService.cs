using EventStore.ClientAPI;
using Marketplace.Infrastructure;

namespace Marketplace;

public class EventStoreService : IHostedService
{
  private readonly IEventStoreConnection _esConnection;
  private readonly ProjectionManager _projectManager;

  public EventStoreService(
    IEventStoreConnection esConnection,
    ProjectionManager projectManager)
  {
    _esConnection = esConnection;
    _projectManager = projectManager;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    await _esConnection.ConnectAsync();
    _projectManager.Start();
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _projectManager.Stop();
    _esConnection.Close();
    return Task.CompletedTask;
  }
}