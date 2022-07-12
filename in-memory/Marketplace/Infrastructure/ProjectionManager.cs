using EventStore.ClientAPI;
using Marketplace.Framework;

namespace Marketplace.Infrastructure;

public class ProjectionManager
{
  private static readonly Serilog.ILogger _log
    = Serilog.Log.ForContext<ProjectionManager>();

  private readonly IEventStoreConnection _connection;
  private readonly IProjection[] _projections;
  private EventStoreAllCatchUpSubscription _subscription = default!;

  public ProjectionManager(IEventStoreConnection connection,
    params IProjection[] projections)
  {
    _connection = connection;
    _projections = projections;
  }

  public void Start()
  {
    var settings = new CatchUpSubscriptionSettings(
      maxLiveQueueSize: 2000,
      readBatchSize: 500,
      verboseLogging: _log.IsEnabled(Serilog.Events.LogEventLevel.Verbose),
      resolveLinkTos: false,
      subscriptionName: "try-out-subscription"
    );

    _subscription = _connection.SubscribeToAllFrom(
      lastCheckpoint: Position.Start,
      settings,
      EventAppeared
    );
  }

  public void Stop() => _subscription.Stop();

  private Task EventAppeared(EventStoreCatchUpSubscription subscription,
    ResolvedEvent resolvedEvent)
  {
    if (resolvedEvent.Event.EventType.Trim().Contains('$'))
    {
      return Task.CompletedTask;
    }

    object @event = resolvedEvent.Deserialize();

    _log.Debug("Projecting event {type}", @event.GetType().Name);

    return Task.WhenAll(_projections.Select(x => x.Project(@event)));
  }
}