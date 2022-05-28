using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class Picture : Entity<PictureId>
{
  public ClassifiedAdId? ParentId { get; private set; }

  public PictureSize Size { get; private set; } = new PictureSize
  {
    Height = 600,
    Width = 800,
  };

  public string Location { get; private set; } = string.Empty;

  public int OrderId { get; private set; }

  public Picture(Action<object> applier) : base(applier)
  {
  }

  internal Picture() { }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.PictureAddedToClassifiedAd e:
        ParentId = new ClassifiedAdId(e.ClassifiedAdId);
        Id = new PictureId(e.PictureId);
        Location = e.Url;
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
      PictureId: Id!,
      Height: newSize.Height,
      Width: newSize.Width
    ));
}