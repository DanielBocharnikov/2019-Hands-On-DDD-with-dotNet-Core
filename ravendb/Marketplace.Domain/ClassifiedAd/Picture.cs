using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class Picture : Entity<PictureId>
{
  internal static Picture NoPicture => new()
  {
    Size = new PictureSize(600, 800),
    Location = default,
    OrderId = default,
  };

  internal PictureSize? Size { get; set; }

  internal Uri? Location { get; set; }

  internal int OrderId { get; set; }

  public Picture(Action<object> applier) : base(applier)
  {
  }

  internal Picture() { }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case Events.PictureAddedToClassifiedAd e:
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
      PictureId: Id!,
      Height: newSize.Height,
      Width: newSize.Width
    ));
}