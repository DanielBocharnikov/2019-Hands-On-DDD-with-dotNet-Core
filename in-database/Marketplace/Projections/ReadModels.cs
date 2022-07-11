namespace Marketplace.Projections;

public static class ReadModels
{
  public record ClassifiedAdDetails(
    Guid ClassifiedAdId,
    Guid SellersId,
    string SellersDisplayName,
    string SellersPhotoUrl,
    string Title,
    decimal Price,
    string CurrencyCode,
    string Description,
    string[] PhotoUrls
  );

  public record UserDetails(Guid UserId, string DisplayName, string PhotoUrl);
}