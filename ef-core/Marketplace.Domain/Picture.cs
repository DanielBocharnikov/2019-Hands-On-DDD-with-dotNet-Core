using Marketplace.Framework;

namespace Marketplace.Domain;

public class Picture : Entity<PictureId>
{
  public Guid PictureId
  {
    get => Id!.Value;
    set { }
  }

  public ClassifiedAdId? ParentId { get; private set; }

  public PictureSize? Size { get; private set; }

  public Uri? Location { get; private set; }

  public int OrderId { get; private set; }

  public Picture(Action<object>? applier) : base(applier)
  {
  }

  private Picture()
  {
  }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.PictureAddedToClassifiedAd e:
        ParentId = new ClassifiedAdId(e.ClassifiedAdId);
        Id = new PictureId(e.PictureId);
        Location = new Uri(e.Url);
        Size = new PictureSize { Height = e.Height, Width = e.Width };
        OrderId = e.OrderId;
        break;
      case Events.ClassifiedAdPictureResized e:
        Size = new PictureSize { Height = e.Height, Width = e.Width };
        break;
      default:
        return;
    }
  }

  public void Resize(PictureSize newSize) =>
    Apply(new Events.ClassifiedAdPictureResized
    (
      ClassifiedAdId: ParentId!.Value,
      PictureId: Id!.Value,
      Height: newSize.Height,
      Width: newSize.Width
    ));
}