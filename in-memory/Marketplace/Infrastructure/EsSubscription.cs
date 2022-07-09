using EventStore.ClientAPI;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Projections;

namespace Marketplace.Infrastructure;

public class EsSubscription
{
  private static readonly Serilog.ILogger _log
    = Serilog.Log.ForContext<EsSubscription>();

  private readonly IEventStoreConnection _connection;
  private readonly IList<ReadModels.ClassifiedAdDetails> _items;
  private EventStoreAllCatchUpSubscription _subscription = default!;

  public EsSubscription(IEventStoreConnection connection, IList<ReadModels.ClassifiedAdDetails> items)
  {
    _connection = connection;
    _items = items;
  }

  public void Start()
  {
    var settings = new CatchUpSubscriptionSettings(
      maxLiveQueueSize: 2000,
      readBatchSize: 500,
      verboseLogging: _log.IsEnabled(Serilog.Events.LogEventLevel.Verbose),
      resolveLinkTos: true,
      subscriptionName: "try-out-subscription"
    );

    _subscription = _connection.SubscribeToAllFrom(
      lastCheckpoint: Position.Start,
      settings,
      EventAppeared
    );
  }

  private Task EventAppeared(
    EventStoreCatchUpSubscription subscription,
    ResolvedEvent resolvedEvent)
  {
    if (resolvedEvent.Event.EventType.Trim().Contains('$')
      || resolvedEvent.OriginalStreamId.Trim().Contains('$'))
    {
      return Task.CompletedTask;
    }

    object @event = resolvedEvent.Deserialize();

    _log.Debug("Projecting event {type}", @event.GetType().Name);

    switch (@event)
    {
      case Events.ClassifiedAdCreated e:
        _items.Add(new ReadModels.ClassifiedAdDetails(
          ClassifiedAdId: e.Id,
          SellerId: e.OwnerId,
          Title: string.Empty,
          Price: decimal.Zero,
          CurrencyCode: string.Empty,
          Description: string.Empty,
          SellerDisplayName: string.Empty,
          Array.Empty<string>()
        ));
        break;

      case Events.ClassifiedAdTitleChanged e:
        UpdateItem(e.Id, ad => ad with
        {
          Title = e.Title
        });
        break;

      case Events.ClassifiedAdTextUpdated e:
        UpdateItem(e.Id, ad => ad with { Description = e.Text });
        break;

      case Events.ClassifiedAdPriceUpdated e:
        UpdateItem(e.Id, ad => ad with
        {
          Price = e.Price,
          CurrencyCode = e.CurrencyCode
        });
        break;

      case Events.PictureAddedToClassifiedAd e:
        UpdateItem(e.ClassifiedAdId, ad => ad with
        {
          PhotoUrls = ad.PhotoUrls.Append(e.Url).ToArray()
        });
        break;

      case Domain.UserProfile.Events.UserDisplayNameUpdated e:
        UpdateMultipleItems(x => x.SellerId == e.UserId,
          x => x with { SellerDisplayName = e.DisplayName });
        break;

      default:
        return Task.CompletedTask;
    }

    return Task.CompletedTask;
  }

  private void UpdateItem(Guid id,
    Func<ReadModels.ClassifiedAdDetails, ReadModels.ClassifiedAdDetails> update)
  {
    ReadModels.ClassifiedAdDetails? item = _items.FirstOrDefault(x =>
      x.ClassifiedAdId == id);

    if (item is null)
    {
      return;
    }

    ReadModels.ClassifiedAdDetails newItem = update(item);
    ReassignItem(currentItem: item, newItem: newItem);
  }

  private void UpdateMultipleItems(
    Func<ReadModels.ClassifiedAdDetails, bool> query,
    Func<ReadModels.ClassifiedAdDetails, ReadModels.ClassifiedAdDetails> update)
  {
    foreach (ReadModels.ClassifiedAdDetails? item in _items.Where(query))
    {
      ReadModels.ClassifiedAdDetails newItem = update(item);
      ReassignItem(item, newItem);
    }
  }

  private void ReassignItem(ReadModels.ClassifiedAdDetails currentItem,
    ReadModels.ClassifiedAdDetails newItem)
  {
    int itemIndex = _items.IndexOf(currentItem);
    _items[itemIndex] = newItem;
  }

  public void Stop() => _subscription.Stop();
}