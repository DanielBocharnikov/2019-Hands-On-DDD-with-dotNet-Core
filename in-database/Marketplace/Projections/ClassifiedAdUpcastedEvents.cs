namespace Marketplace.Projections;

public static class ClassifiedAdUpcastedEvents
{
  public static class V1
  {
    public record ClassifiedAdPublished(
      Guid Id,
      Guid OwnerId,
      string SellersPhotoUrl,
      Guid ApprovedBy
    );
  }
}