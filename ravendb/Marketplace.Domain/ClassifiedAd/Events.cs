namespace Marketplace.Domain.ClassifiedAd;

public static class Events
{
  public record ClassifiedAdCreated(Guid Id, Guid OwnerId);

  public record ClassifiedAdTitleChanged(Guid Id, string Title);

  public record ClassifiedAdTextUpdated(Guid Id, string Text);

  public record ClassifiedAdPriceUpdated(Guid Id, decimal Price,
    string CurrencyCode);

  public record ClassifiedAdSentToReview(Guid Id);

  public record PictureAddedToClassifiedAd(Guid ClassifiedAdId, Guid PictureId,
    string Url, int Height, int Width, int OrderId);

  public record ClassifiedAdPictureResized(Guid PictureId, int Height,
    int Width);
}