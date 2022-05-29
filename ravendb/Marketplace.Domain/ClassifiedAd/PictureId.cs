using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class PictureId : ValueObject
{
  public static PictureId None => new() { Value = Guid.Empty };

  public Guid Value { get; internal set; }

  public PictureId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(
        paramName: nameof(value),
        message: "Picture identity cannot be empty"
      );
    }

    Value = value;
  }

  /// <summary>
  /// Ctor used for reapplying events from aggregate root and satisfies
  /// serialization requirements.
  /// </summary>
  internal PictureId()
  {
  }

  public static implicit operator string(PictureId self)
    => self.Value.ToString();

  public static implicit operator PictureId(string value)
    => new(Guid.Parse(value));

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }
}