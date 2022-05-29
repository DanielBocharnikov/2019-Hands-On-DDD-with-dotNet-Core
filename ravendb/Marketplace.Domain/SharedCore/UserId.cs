using Marketplace.Framework;

namespace Marketplace.Domain.SharedCore;

public sealed class UserId : ValueObject
{
  public static UserId None => new() { Value = Guid.Empty };

  public Guid Value { get; internal set; }

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

  /// <summary>
  /// Ctor used for reapplying events from aggregate root and satisfies
  /// serialization requirements.
  /// </summary>
  internal UserId()
  {
  }

  public static implicit operator Guid(UserId self) => self.Value;

  public static implicit operator UserId(string value)
    => new(Guid.Parse(value));

  public override string ToString() => Value.ToString();

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Value;
  }
}
