using Marketplace.Framework;

namespace Marketplace.Domain;

public class ClassifiedAdId : ValueObject
{
  private readonly Guid _value;

  public ClassifiedAdId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(
        paramName: nameof(value),
        message: "Classified Ad identity cannot be empty"
      );
    }

    _value = value;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return _value;
  }

  public static implicit operator Guid(ClassifiedAdId self)
    => self._value;

  public override string ToString() => _value.ToString();
}
