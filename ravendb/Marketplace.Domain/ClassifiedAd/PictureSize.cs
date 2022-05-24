using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class PictureSize : ValueObject
{
  public int Height { get; internal set; }

  public int Width { get; internal set; }

  public PictureSize(int height, int width)
  {
    if (height <= 0)
    {
      throw new ArgumentOutOfRangeException(
        nameof(height),
        "Picture height must be a positive number"
      );
    }

    if (width <= 0)
    {
      throw new ArgumentOutOfRangeException(
        nameof(width),
        "Picture width must be a positive number"
      );
    }

    Height = height;
    Width = width;
  }

  internal PictureSize()
  {
  }

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Height;
    yield return Width;
  }
}