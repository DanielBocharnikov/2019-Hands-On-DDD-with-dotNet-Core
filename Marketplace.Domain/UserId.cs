using Marketplace.Framework;

namespace Marketplace.Domain;

public class UserId : ValueObject
{
  private readonly Guid _value;

  public UserId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(
        paramName: nameof(value),
        message: "User identity cannot be empty"
      );
    }

    _value = value;
  }

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return _value;
  }

  public static implicit operator Guid(UserId self) => self._value;
}
