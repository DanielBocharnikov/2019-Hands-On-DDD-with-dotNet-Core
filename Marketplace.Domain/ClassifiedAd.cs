using Marketplace.Framework;

namespace Marketplace.Domain;

public class ClassifiedAd : Entity
{
  public ClassifiedAdId? Id { get; private set; }
  public UserId? OwnerId { get; private set; }
  public ClassifiedAdTitle? Title { get; private set; }
  public ClassifiedAdText? Text { get; private set; }
  public Price? Price { get; private set; }
  public ClassifiedAdState State { get; private set; }
  public UserId? ApprovedBy { get; private set; }

  public enum ClassifiedAdState
  {
    PendingReview = 0,
    Active = 1,
    Inactive = 2,
    MarkedAsSold = 3
  }

  public ClassifiedAd(ClassifiedAdId id, UserId ownerId) =>
    Apply(new Events.ClassifiedAdCreated(id, ownerId));

  public void SetTitle(ClassifiedAdTitle title) =>
    Apply(new Events.ClassifiedAdTitleChanged(Id!, title));

  public void UpdateText(ClassifiedAdText text) =>
    Apply(new Events.ClassifiedAdTextUpdated(Id!, text));

  public void UpdatePrice(Price price) =>
    Apply(
      new Events.ClassifiedAdPriceUpdated(
        Id!,
        price.Amount,
        price.Currency.CurrencyCode
      )
    );

  public void RequestToPublish() =>
    Apply(new Events.ClassifiedAdSentToReview(Id!));

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.ClassifiedAdCreated e:
        Id = new ClassifiedAdId(e.Id);
        OwnerId = new UserId(e.OwnerId);
        State = ClassifiedAdState.Inactive;
        break;
      case Events.ClassifiedAdTitleChanged e:
        Title = new ClassifiedAdTitle(e.Title);
        break;
      case Events.ClassifiedAdTextUpdated e:
        Text = new ClassifiedAdText(e.Text);
        break;
      case Events.ClassifiedAdPriceUpdated e:
        Price = new Price(e.Price, e.CurrencyCode);
        break;
      case Events.ClassifiedAdSentToReview:
        State = ClassifiedAdState.PendingReview;
        break;
    }
  }

  protected override void EnsureValidState()
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