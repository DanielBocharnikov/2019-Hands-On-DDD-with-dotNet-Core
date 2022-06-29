using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class PictureSize : ValueObject
{
  public static PictureSize None => new()
  {
    Height = 0,
    Width = 0
  };

  public int Height { get; internal set; }

  public int Width { get; internal set; }

  /// <summary>
  /// Ctor to safely create PictureSize value object elsewhere.
  /// </summary>
  /// <param name="height"></param>
  /// <param name="width"></param>
  public PictureSize(int height, int width)
  {
    if (height <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(height),
        "Picture height must be a positive number"
      );
    }

    if (width <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(width),
        "Picture width must be a positive number"
      );
    }

    Height = height;
    Width = width;
  }

  /// <summary>
  /// Ctor used for reapplying events from aggregate root and satisfies
  /// serialization requirements.
  /// </summary>
  internal PictureSize()
  {
  }

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Height;
    yield return Width;
  }
}