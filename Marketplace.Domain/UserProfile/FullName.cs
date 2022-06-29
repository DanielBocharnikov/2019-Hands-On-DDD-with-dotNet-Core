using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public sealed class FullName : ValueObject
{
  public static FullName None => new() { };

  public string Value { get; init; } = string.Empty;

  internal FullName(string value) => Value = value;

  private FullName()
  {
  }

  public static FullName FromString(string fullName)
  {
    if (fullName.IsEmpty())
    {
      throw new ArgumentNullException(nameof(fullName));
    }

    return new(fullName);
  }

  public static implicit operator string(FullName fullName) => fullName.Value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }
}