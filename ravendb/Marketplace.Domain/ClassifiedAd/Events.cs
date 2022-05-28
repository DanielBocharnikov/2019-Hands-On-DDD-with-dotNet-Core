namespace Marketplace.Domain.ClassifiedAd;

public static class Events
{
  public record class ClassifiedAdCreated(Guid Id, Guid OwnerId);

  public record class ClassifiedAdTitleChanged(Guid Id, string Title);

  public record class ClassifiedAdTextUpdated(Guid Id, string Text);

  public record ClassifiedAdPriceUpdated(
    Guid Id,
    decimal Price,
    string CurrencyCode,
    bool InUse,
    int DecimalPlaces);

  public record class ClassifiedAdSentToReview(Guid Id);

  public record class ClassifiedAdPublished(Guid Id, Guid ApprovedBy);

  public record class PictureAddedToClassifiedAd(
    Guid ClassifiedAdId,
    Guid PictureId,
    string Url,
    int Height,
    int Width,
    int OrderId);

  public record class ClassifiedAdPictureResized(
    Guid ClassifiedAdId,
    Guid PictureId,
    int Height,
    int Width);
}