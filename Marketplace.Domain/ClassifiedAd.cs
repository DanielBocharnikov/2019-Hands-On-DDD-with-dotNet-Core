using Marketplace.Framework;

namespace Marketplace.Domain;

public class ClassifiedAd : Entity
{
  public ClassifiedAdId Id { get; init; }
  public UserId OwnerId { get; init; }
  public ClassifiedAdTitle? Title { get; private set; }
  public ClassifiedAdText? Text { get; private set; }
  public Price? Price { get; private set; }
  public ClassifiedAdState State { get; private set; }
  public UserId? ApprovedBy { get; }

  public enum ClassifiedAdState
  {
    PendingReview = 0,
    Active = 1,
    Inactive = 2,
    MarkedAsSold = 3
  }

  public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
  {
    Id = id;
    OwnerId = ownerId;
    State = ClassifiedAdState.Inactive;
    EnsureValidState();
    Raise(new Events.ClassifiedAdCreated(id, ownerId));
  }

  public void SetTitle(ClassifiedAdTitle title)
  {
    Title = title;
    EnsureValidState();
    Raise(new Events.ClassifiedAdTitleChanged(Id, Title));
  }

  public void UpdateText(ClassifiedAdText text)
  {
    Text = text;
    EnsureValidState();
    Raise(new Events.ClassifiedAdTextUpdated(Id, Text));
  }

  public void UpdatePrice(Price price)
  {
    Price = price;
    EnsureValidState();
    Raise(new Events.ClassifiedAdPriceUpdated(
      Id,
      Price.Amount,
      Price.Currency.CurrencyCode));
  }

  public void RequestToPublish()
  {
    State = ClassifiedAdState.PendingReview;
    EnsureValidState();
    Raise(new Events.ClassifiedAdSentToReview(Id));
  }
  private void EnsureValidState()
  {
    bool valid =
      Id is not null
      && OwnerId is not null
      && (State switch
      {
        ClassifiedAdState.PendingReview =>
          Title is not null
          && Text is not null
          && Price?.Amount > decimal.Zero,
        ClassifiedAdState.Active =>
          Title is not null
          && Text is not null
          && Price?.Amount > decimal.Zero
          && ApprovedBy is not null,
        ClassifiedAdState.Inactive => true,
        ClassifiedAdState.MarkedAsSold => true,
        _ => true
      });

    if (!valid)
    {
      throw new InvalidEntityStateException(
        this,
        $"Post-checks failed in state {State}");
    }
  }
}