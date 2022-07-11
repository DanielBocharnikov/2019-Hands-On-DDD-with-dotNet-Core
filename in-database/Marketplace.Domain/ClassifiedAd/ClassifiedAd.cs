using Marketplace.Domain.SharedCore;
using Marketplace.Framework;
using static Marketplace.Domain.ClassifiedAd.Events;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAd : AggregateRoot<ClassifiedAdId>
{
  public UserId OwnerId { get; private set; } = UserId.None;

  public ClassifiedAdTitle Title { get; private set; } = ClassifiedAdTitle.None;

  public ClassifiedAdText Text { get; private set; } = ClassifiedAdText.None;

  public Price Price { get; private set; } = Price.None;

  public IEnumerable<Picture> Pictures => _pictures.AsEnumerable();

  public Picture FirstPicture => Pictures.OrderBy(x => x.OrderId)
    .FirstOrDefault() ?? Picture.None;

  public ClassifiedAdState State { get; private set; }

  public UserId ApprovedBy { get; private set; } = UserId.None;

  public enum ClassifiedAdState
  {
    PendingReview = 0,
    Active = 1,
    Inactive = 2,
    MarkedAsSold = 3
  }

  private readonly List<Picture> _pictures = new();

  private ClassifiedAd()
  {
  }

  public ClassifiedAd(ClassifiedAdId id, UserId ownerId)
    => Apply(new ClassifiedAdCreated(Id: id, OwnerId: ownerId));

  public void SetTitle(ClassifiedAdTitle title)
    => Apply(new ClassifiedAdTitleChanged(Id: Id, Title: title));

  public void UpdateText(ClassifiedAdText text)
    => Apply(new ClassifiedAdTextUpdated(Id: Id, Text: text));

  public void UpdatePrice(Price price) =>
    Apply(
      new ClassifiedAdPriceUpdated(
        Id: Id,
        Price: price.Amount,
        CurrencyCode: price.Currency.CurrencyCode,
        InUse: price.Currency.InUse,
        DecimalPlaces: price.Currency.DecimalPlaces
      )
    );

  public void AddPicture(Uri pictureUri, PictureSize size)
  {
    Apply(new PictureAddedToClassifiedAd(
      ClassifiedAdId: Id,
      PictureId: Guid.NewGuid(),
      Url: pictureUri.ToString(),
      Height: size.Height,
      Width: size.Width,
      OrderId: NewPictureOrder()));

    int NewPictureOrder()
      => Pictures.Any() ? Pictures.Max(x => x.OrderId) + 1 : 0;
  }

  public void ResizePicture(PictureId pictureId, PictureSize newSize)
  {
    Picture picture = FindPicture(pictureId);

    if (picture == Picture.None)
    {
      throw new InvalidOperationException(
        "Cannot resize a picture that doesn't exists.");
    }

    picture.Resize(newSize);
  }

  public void RequestToPublish()
    => Apply(new ClassifiedAdSentToReview(Id: Id));

  public void Publish(UserId userId)
    => Apply(new ClassifiedAdPublished(
      Id: Id,
      OwnerId: OwnerId,
      ApprovedBy: userId));

  protected override void When(object @event)
  {
    Picture picture;

    switch (@event)
    {
      case ClassifiedAdCreated e:
        Id = new ClassifiedAdId { Value = e.Id };
        OwnerId = new UserId { Value = e.OwnerId };
        State = ClassifiedAdState.Inactive;
        break;

      case ClassifiedAdTitleChanged e:
        Title = new ClassifiedAdTitle(e.Title);
        break;

      case ClassifiedAdTextUpdated e:
        Text = new ClassifiedAdText(e.Text);
        break;

      case ClassifiedAdPriceUpdated e:
        Price = new Price(e.Price, e.CurrencyCode, e.InUse, e.DecimalPlaces);
        break;

      case PictureAddedToClassifiedAd e:
        picture = new Picture(Apply);
        ApplyToEntity(picture, e);
        _pictures.Add(picture);
        break;

      case ClassifiedAdPictureResized e:
        picture = FindPicture((PictureId)e.PictureId);
        ApplyToEntity(picture, e);
        break;

      case ClassifiedAdSentToReview:
        State = ClassifiedAdState.PendingReview;
        break;

      case ClassifiedAdPublished e:
        ApprovedBy = new UserId(e.ApprovedBy);
        State = ClassifiedAdState.Active;
        break;

      default:
        return;
    }
  }

  protected override void EnsureValidState()
  {
    bool valid = Id != ClassifiedAdId.None && OwnerId != UserId.None
      && (State switch
      {
        ClassifiedAdState.PendingReview =>
          Title != ClassifiedAdTitle.None
          && Text != ClassifiedAdText.None
          && Price != Price.None
          && Price.Amount > decimal.Zero
          && Price.Currency != CurrencyDetails.None
          && Price.Currency.CurrencyCode != string.Empty
          && FirstPicture != Picture.None
          && FirstPicture.HasCorrectSize(),

        ClassifiedAdState.Active =>
          Title != ClassifiedAdTitle.None
          && Text != ClassifiedAdText.None
          && Price != Price.None
          && Price.Amount > decimal.Zero
          && Price.Currency != CurrencyDetails.None
          && Price.Currency.CurrencyCode != string.Empty
          && FirstPicture != Picture.None
          && FirstPicture.HasCorrectSize()
          && ApprovedBy != UserId.None,

        ClassifiedAdState.Inactive => true,

        ClassifiedAdState.MarkedAsSold => true,

        _ => true
      });

    if (!valid)
    {
      throw new DomainExceptions.InvalidEntityStateException(this,
        $"Post-checks failed in state {State}");
    }
  }

  private Picture FindPicture(PictureId id)
    => _pictures.Find(x => x.Id == id) ?? Picture.None;
}