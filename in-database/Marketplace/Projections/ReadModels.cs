namespace Marketplace.Projections;

public static class ReadModels
{
  public record ClassifiedAdDetails
  {
    public string Id { get; set; } = string.Empty;
    public Guid SellersId { get; set; }
    public string SellersDisplayName { get; set; } = string.Empty;
    public string SellersPhotoUrl { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] PhotoUrls { get; set; } = Array.Empty<string>();
  }

  public record UserDetails
  {
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
  }
}