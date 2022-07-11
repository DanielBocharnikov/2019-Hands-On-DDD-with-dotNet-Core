using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public class ClassifiedAdId : ValueObject
{
  /// <summary>
  /// Factory method to represent that ClassifiedAdId is absent.
  /// </summary>
  public static ClassifiedAdId None => new() { Value = Guid.Empty };

  public Guid Value { get; internal set; }

  public ClassifiedAdId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(paramName: nameof(value),
        message: "Classified Ad identity cannot be empty"
      );
    }

    Value = value;
  }

  /// <summary>
  /// Ctor used for reapplying events from aggregate root and satisfies
  /// serialization requirements.
  /// </summary>
  internal ClassifiedAdId()
  {
  }

  public static implicit operator Guid(ClassifiedAdId self)
    => self.Value;

  public static explicit operator ClassifiedAdId(Guid value)
  => new(value);

  public static explicit operator ClassifiedAdId(string value)
    => new(Guid.Parse(value));

  public override string ToString() => Value.ToString();

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value;
  }
}
