namespace Marketplace.ClassifiedAd;

public static class QueryModels
{
  public record GetPublishedClassifiedAds(int Page, int PageSize);

  public record GetOwnersClassifiedAd(Guid OwnerId, int Page, int PageSize);

  public record GetPublicClassifiedAd(Guid ClassifiedAdId);
}