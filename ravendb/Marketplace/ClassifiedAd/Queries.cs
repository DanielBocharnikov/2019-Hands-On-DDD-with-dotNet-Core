using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using static Marketplace.ClassifiedAd.ReadModels;
using static Marketplace.Domain.ClassifiedAd.ClassifiedAd;

namespace Marketplace.ClassifiedAd;

public static class Queries
{
  public static Task<List<PublicClassifiedAdListItem>> Query(
    this IAsyncDocumentSession session,
    QueryModels.GetPublishedClassifiedAds query)
      => session.Query<Domain.ClassifiedAd.ClassifiedAd>()
        .Where(x => x.State == ClassifiedAdState.Active)
        .Select(x => new PublicClassifiedAdListItem
        {
          ClassifiedAdId = x.Id.Value,
          Title = x.Title.Value,
          Price = x.Price.Amount,
          CurrencyCode = x.Price.Currency.CurrencyCode,
          PhotoUrl = x.FirstPicture.Location
        })
        .PagedList(query.Page, query.PageSize);

  public static Task<List<PublicClassifiedAdListItem>> Query(
    this IAsyncDocumentSession session, QueryModels.GetOwnersClassifiedAd query)
      => session.Query<Domain.ClassifiedAd.ClassifiedAd>()
        .Where(x => x.OwnerId!.Value == query.OwnerId)
        .Select(x => new PublicClassifiedAdListItem
        {
          ClassifiedAdId = x.Id.Value,
          Price = x.Price.Amount,
          Title = x.Title.Value,
          CurrencyCode = x.Price.Currency.CurrencyCode,
          PhotoUrl = x.FirstPicture.Location
        })
        .PagedList(query.Page, query.PageSize);

  public static Task<ClassifiedAdDetails> Query(
    this IAsyncDocumentSession session, QueryModels.GetPublicClassifiedAd query)
      => (from ad in session.Query<Domain.ClassifiedAd.ClassifiedAd>()
          where ad.Id.Value == query.ClassifiedAdId
          let user = RavenQuery.Load<Domain.UserProfile.UserProfile>(
            "UserProfile/" + ad.OwnerId.Value
          )
          select new ClassifiedAdDetails
          {
            ClassifiedAdId = ad.Id.Value,
            Title = ad.Title.Value,
            Description = ad.Text.Value,
            Price = ad.Price.Amount,
            CurrencyCode = ad.Price.Currency.CurrencyCode,
            SellerDisplayName = user.DisplayName.Value,
            PhotoUrls = ad.Pictures.Select(x => x.Location).ToArray()
          })
        .SingleAsync();

  private static Task<List<T>> PagedList<T>(this IRavenQueryable<T> query,
    int page, int pageSize) => query.Skip(page * pageSize).Take(pageSize)
      .ToListAsync();
}