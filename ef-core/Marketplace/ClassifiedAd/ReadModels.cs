namespace Marketplace.ClassifiedAd;

public static class ReadModels
{
  public class ClassifiedAdDetails
  {
    public Guid ClassifiedAdId { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SellerDisplayName { get; set; } = string.Empty;
    public string[] PhotoUrls { get; set; } = Array.Empty<string>();
  }

  public class PublicClassifiedAdListItem
  {
    public Guid ClassifiedAdId { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string PhotoUrl { get; set; } = string.Empty;
  }
}