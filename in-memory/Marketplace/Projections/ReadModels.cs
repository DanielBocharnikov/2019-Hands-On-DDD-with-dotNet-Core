namespace Marketplace.Projections;

public static class ReadModels
{
  public record ClassifiedAdDetails(
    Guid ClassifiedAdId,
    Guid SellerId,
    string Title,
    decimal Price,
    string CurrencyCode,
    string Description,
    string SellerDisplayName,
    string[] PhotoUrls
  );

  public record PublicClassifiedAdListItem(
    Guid ClassifiedAdId,
    string Title,
    decimal Price,
    string CurrencyCode,
    string PhotoUrl
  );
}