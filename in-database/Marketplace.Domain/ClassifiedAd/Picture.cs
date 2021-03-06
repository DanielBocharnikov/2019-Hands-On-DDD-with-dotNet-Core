using Marketplace.Framework;
using static Marketplace.Domain.ClassifiedAd.Events;

namespace Marketplace.Domain.ClassifiedAd;

public class Picture : Entity<PictureId>
{
  public static readonly Picture None = new()
  {
    Id = PictureId.None,
    ParentId = ClassifiedAdId.None,
    Size = PictureSize.None,
    Location = string.Empty,
    OrderId = default,
  };

  public ClassifiedAdId ParentId { get; private set; } = ClassifiedAdId.None;

  public PictureSize Size { get; private set; } = PictureSize.None;

  public string Location { get; private set; } = string.Empty;

  public int OrderId { get; private set; }

  public Picture(Action<object> applier) : base(applier)
  {
  }

  /// <summary>
  /// Ctor used for reapplying events from aggregate root and satisfies
  /// serialization requirements.
  /// </summary>
  internal Picture() { }

  protected override void When(object @event)
  {
    switch (@event)
    {
      case PictureAddedToClassifiedAd e:
        ParentId = new ClassifiedAdId(e.ClassifiedAdId);
        Id = new PictureId { Value = e.PictureId };
        Location = e.Url;
        Size = new PictureSize { Height = e.Height, Width = e.Width };
        OrderId = e.OrderId;
        break;

      case ClassifiedAdPictureResized e:
        Size = new PictureSize { Height = e.Height, Width = e.Width };
        break;

      default:
        return;
    }
  }

  public void Resize(PictureSize newSize) => Apply(
    new ClassifiedAdPictureResized
    (
      ClassifiedAdId: ParentId.Value,
      PictureId: Id.Value,
      Height: newSize.Height,
      Width: newSize.Width
    ));
}