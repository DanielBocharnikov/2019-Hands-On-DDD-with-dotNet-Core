using Marketplace.Framework;

namespace Marketplace.Domain;

public class UserId : ValueObject
{
  public Guid Value { get; init; }

  public static readonly UserId NoUser = new();

  public UserId(Guid value)
  {
    if (value == default)
    {
      throw new ArgumentNullException(
        paramName: nameof(value),
        message: "User identity cannot be empty"
      );
    }

    Value = value;
  }

  public static implicit operator Guid(UserId self) => self.Value;

  public static implicit operator UserId(string value)
    => new(Guid.Parse(value));

  public override string ToString() => Value.ToString();

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value;
  }

  private UserId()
  {
  }
}
