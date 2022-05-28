namespace Marketplace.ClassifiedAd;

public static class Commands
{
  public static class V1
  {
    public record class Create(Guid Id, Guid OwnerId);

    public record class SetTitle(Guid Id, string Title);

    public record class UpdateText(Guid Id, string Text);

    public record class UpdatePrice(
      Guid Id,
      decimal Price,
      string CurrencyCode);

    public record class RequestToPublish(Guid Id);

    public record class Publish(Guid Id, Guid ApprovedBy);
  }
}