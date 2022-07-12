namespace Marketplace.Projections;

public static class ReadModels
{
  public record ClassifiedAdDetails(
    string Id,
    Guid SellersId,
    string SellersDisplayName,
    string SellersPhotoUrl,
    string Title,
    decimal Price,
    string CurrencyCode,
    string Description,
    string[] PhotoUrls
  );

  public record UserDetails(string Id, string DisplayName, string PhotoUrl);
}