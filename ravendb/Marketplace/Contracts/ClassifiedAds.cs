namespace Marketplace.Contracts;

public static class ClassifiedAds
{
  public static class V1
  {
    public record Create(Guid Id, Guid OwnerId);
    public record SetTitle(Guid Id, string Title);
    public record UpdateText(Guid Id, string Text);
    public record UpdatePrice(Guid Id, decimal Price, string CurrencyCode);
    public record RequestToPublish(Guid Id);
  }
}