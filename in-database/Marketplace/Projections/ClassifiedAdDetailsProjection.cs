using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.ClassifiedAd.Events;
using static Marketplace.Domain.UserProfile.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.Projections;

public class ClassifiedAdDetailsProjection
  : RavenDbProjection<ClassifiedAdDetails>
{
  private readonly Func<Guid, Task<string>> _getUserDisplayName;

  public ClassifiedAdDetailsProjection(
    Func<IAsyncDocumentSession> getSession,
    Func<Guid, Task<string>> getUserDisplayName
  ) : base(getSession) => _getUserDisplayName = getUserDisplayName;

  public override Task Project(object @event)
    => @event switch
    {
      ClassifiedAdCreated e => Create(
        async () => new ClassifiedAdDetails(
          Id: e.Id.ToString(),
          SellersId: e.OwnerId,
          SellersDisplayName: await _getUserDisplayName(e.OwnerId),
          SellersPhotoUrl: string.Empty,
          Title: string.Empty,
          Price: decimal.Zero,
          CurrencyCode: string.Empty,
          Description: string.Empty,
          PhotoUrls: Array.Empty<string>()
        )
      ),
      ClassifiedAdTitleChanged e => UpdateOne(
        id: e.Id,
        update: ad => ad = ad with { Title = e.Title }
      ),
      ClassifiedAdTextUpdated e => UpdateOne(
        id: e.Id,
        update: ad => ad = ad with { Description = e.Text }
      ),
      ClassifiedAdPriceUpdated e => UpdateOne(
        id: e.Id,
        update: ad => ad = ad with
        {
          Price = e.Price,
          CurrencyCode = e.CurrencyCode
        }
      ),
      PictureAddedToClassifiedAd e => UpdateOne(
        id: e.ClassifiedAdId,
        update: ad => ad = ad with
        {
          PhotoUrls = ad.PhotoUrls.Append(e.Url).ToArray()
        }
      ),
      UserDisplayNameUpdated e => UpdateWhere(
        where: ad => ad.SellersId == e.UserId,
        update: ad => ad = ad with { SellersDisplayName = e.DisplayName }
      ),
      V1.ClassifiedAdPublished e => UpdateOne(
        id: e.Id,
        update: ad => ad = ad with { SellersPhotoUrl = e.SellersPhotoUrl }
      ),
      _ => Task.CompletedTask
    };
}