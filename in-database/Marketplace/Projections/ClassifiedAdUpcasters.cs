using EventStore.ClientAPI;
using Marketplace.Framework;
using Marketplace.Infrastructure;
using static Marketplace.Domain.ClassifiedAd.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;

namespace Marketplace.Projections;

public class ClassifiedAdUpcasters : IProjection
{
  private readonly IEventStoreConnection _eventStoreConnection;
  private readonly Func<Guid, Task<string>> _getUserPhoto;
  private const string StreamName = "UpcastedClassifiedAdEvents";

  public ClassifiedAdUpcasters(IEventStoreConnection eventStoreConnection, Func<Guid, Task<string>> getUserPhoto)
  {
    _eventStoreConnection = eventStoreConnection;
    _getUserPhoto = getUserPhoto;
  }

  public async Task Project(object resolvedEvent)
  {
    switch (resolvedEvent)
    {
      case ClassifiedAdPublished e:
        string photoUrl = await _getUserPhoto(e.OwnerId);

        V1.ClassifiedAdPublished newEvent = new(
          e.Id,
          e.OwnerId,
          SellersPhotoUrl: photoUrl,
          e.ApprovedBy
        );

        await _eventStoreConnection.AppendEvents(
          StreamName,
          ExpectedVersion.Any,
          newEvent
        );
        break;
      default:
        return;
    }
  }
}