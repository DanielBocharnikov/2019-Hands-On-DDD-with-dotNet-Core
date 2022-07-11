using EventStore.ClientAPI;
using Marketplace.Framework;

namespace Marketplace.Infrastructure;

public class ProjectionManager
{
  private static readonly Serilog.ILogger _log
    = Serilog.Log.ForContext<ProjectionManager>();
  private readonly IEventStoreConnection _connection;
  private readonly ICheckpointStore _checkpointStore;
  private readonly IProjection[] _projections;
  private EventStoreAllCatchUpSubscription _subscription = default!;

  public ProjectionManager(
    IEventStoreConnection connection,
    ICheckpointStore checkpointStore,
    params IProjection[] projections
  )
  {
    _connection = connection;
    _checkpointStore = checkpointStore;
    _projections = projections;
  }

  public async void Start()
  {
    var settings = new CatchUpSubscriptionSettings(
      maxLiveQueueSize: 2000,
      readBatchSize: 500,
      verboseLogging: _log.IsEnabled(Serilog.Events.LogEventLevel.Verbose),
      resolveLinkTos: false,
      subscriptionName: "try-out-subscription"
    );

    Position position = await _checkpointStore.GetCheckpoint();

    _subscription = _connection.SubscribeToAllFrom(
      lastCheckpoint: position,
      settings,
      EventAppeared
    );
  }

  public void Stop() => _subscription.Stop();

  private async Task EventAppeared(EventStoreCatchUpSubscription subscription,
    ResolvedEvent resolvedEvent)
  {
    if (resolvedEvent.Event.EventType.Trim().Contains('$'))
    {
      return;
    }

    object @event = resolvedEvent.Deserialize();

    _log.Debug("Projecting event {type}", @event.GetType().Name);

    await Task.WhenAll(_projections.Select(x => x.Project(@event)));

    await _checkpointStore
      .StoreCheckpoint(resolvedEvent.OriginalPosition);
  }
}