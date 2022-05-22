using Marketplace.Framework;

namespace Marketplace.Domain;

public class ClassifiedAdId : ValueObject
{
  public Guid Value { get; init; }

  public ClassifiedAdId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(
        paramName: nameof(value),
        message: "Classified Ad identity cannot be empty"
      );
    }

    Value = value;
  }

  private ClassifiedAdId()
  {
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value;
  }

  public static implicit operator Guid(ClassifiedAdId self)
    => self.Value;

  public static implicit operator ClassifiedAdId(string value)
    => new(Guid.Parse(value));

  public override string ToString() => Value.ToString();
}
