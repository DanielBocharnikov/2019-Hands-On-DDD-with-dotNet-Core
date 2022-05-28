using Marketplace.Domain.SharedCore;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
{
  private string DbId
  {
    get => $"ClassifiedAd/{Id!.Value}";
    set { }
  }
  private readonly List<Picture> _pictures = new();

  public UserId? OwnerId { get; private set; }

  public ClassifiedAdTitle? Title { get; private set; }

  public ClassifiedAdText? Text { get; private set; }

  public Price? Price { get; private set; }

  public IEnumerable<Picture> Pictures => _pictures.AsEnumerable();

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
    Apply(new Events.ClassifiedAdCreated(Id: id, OwnerId: ownerId));

  public void SetTitle(ClassifiedAdTitle title) =>
    Apply(new Events.ClassifiedAdTitleChanged(Id: Id!, Title: title));

  public void UpdateText(ClassifiedAdText text) =>
    Apply(new Events.ClassifiedAdTextUpdated(Id: Id!, Text: text));

  public void UpdatePrice(Price price) =>
    Apply(
      new Events.ClassifiedAdPriceUpdated(
        Id: Id!,
        Price: price.Amount,
        CurrencyCode: price.Currency.CurrencyCode,
        InUse: price.Currency.InUse,
        DecimalPlaces: price.Currency.DecimalPlaces
      )
    );

  public void RequestToPublish() =>
    Apply(new Events.ClassifiedAdSentToReview(Id: Id!));

  public void Publish(UserId userId)
    => Apply(new Events.ClassifiedAdPublished(
      Id: Id!,
      ApprovedBy: userId));

  public void AddPicture(Uri pictureUri, PictureSize size) =>
    Apply(new Events.PictureAddedToClassifiedAd(
      ClassifiedAdId: Id!,
      PictureId: new Guid(),
      Url: pictureUri.ToString(),
      Height: size.Height,
      Width: size.Width,
      OrderId: _pictures.Max(x => x.OrderId) + 1
    ));

  public void ResizePicture(PictureId pictureId, PictureSize newSize)
  {
    Picture? picture = FindPicture(pictureId);

    if (picture is null)
    {
      throw new InvalidOperationException(
        "Cannot resize a picture that doesn't exists.");
    }

    picture.Resize(newSize);
  }

  private Picture? FindPicture(PictureId id) =>
    _pictures.Find(x => x.Id! == id);

  protected override void When(object @event)
  {
    Picture picture;

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
        Price = new Price(e.Price, e.CurrencyCode, e.InUse, e.DecimalPlaces);
        break;
      case Events.ClassifiedAdSentToReview:
        State = ClassifiedAdState.PendingReview;
        break;
      case Events.ClassifiedAdPublished e:
        ApprovedBy = new UserId(e.ApprovedBy);
        State = ClassifiedAdState.Active;
        break;
      case Events.PictureAddedToClassifiedAd e:
        picture = new Picture(Apply);
        ApplyToEntity(picture, @event);
        _pictures.Add(picture);
        break;
      default:
        return;
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
          && Price!.Amount > decimal.Zero,
        ClassifiedAdState.Active =>
          Title is not null
          && Text is not null
          && Price!.Amount > decimal.Zero
          && ApprovedBy is not null,
        ClassifiedAdState.Inactive => true,
        ClassifiedAdState.MarkedAsSold => true,
        _ => true
      });

    if (!valid)
    {
      throw new DomainExceptions.InvalidEntityStateException(
        this,
        $"Post-checks failed in state {State}");
    }
  }
}