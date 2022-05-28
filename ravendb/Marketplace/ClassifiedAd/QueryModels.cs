namespace Marketplace.ClassifiedAd;

public static class QueryModels
{
  public record class GetPublishedClassifiedAds(int Page, int PageSize);

  public record class GetOwnersClassifiedAd(
    Guid OwnerId,
    int Page,
    int PageSize);

  public record class GetPublicClassifiedAd(Guid ClassifiedAdId);
}