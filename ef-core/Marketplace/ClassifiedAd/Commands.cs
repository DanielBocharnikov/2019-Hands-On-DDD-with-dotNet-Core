namespace Marketplace.ClassifiedAd;

public static class ClassifiedAds
{
  public static class V1
  {
    public record Create(Guid Id, Guid OwnerId);

    public record SetTitle(Guid Id, string Title);

    public record UpdateText(Guid Id, string Text);

    public record UpdatePrice(Guid Id, decimal Price, string CurrencyCode);

    public record class AddPicture(Guid Id, string PictureUrl, int Height,
      int Width);

    public record class ResizePicture(Guid Id, Guid PictureId, int Height,
      int Width);

    public record class RequestToPublish(Guid Id);
    public record class Publish(Guid Id, Guid ApprovedBy);
  }
}