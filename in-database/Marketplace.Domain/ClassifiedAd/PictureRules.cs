namespace Marketplace.Domain.ClassifiedAd;

public static class PictureRules
{
  public static bool HasCorrectSize(this Picture picture)
  {
    if (picture is null)
    {
      throw new ArgumentNullException(nameof(picture),
        "Picture cannot be null");
    }

    return picture.Size.Width >= 800 && picture.Size.Height >= 600;
  }
}