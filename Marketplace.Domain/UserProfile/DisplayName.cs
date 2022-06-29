using Marketplace.Domain.SharedCore;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile;

public sealed class DisplayName : ValueObject
{
  public static DisplayName None => new() { };

  public string Value { get; init; } = string.Empty;

  internal DisplayName(string displayName) => Value = displayName;

  private DisplayName()
  {
  }

  public static DisplayName FromString(string displayName,
    CheckTextForProfanity hasProfanity)
  {
    if (displayName.IsEmpty())
    {
      throw new ArgumentNullException(nameof(displayName));
    }

    if (hasProfanity(displayName))
    {
      throw new DomainExceptions.ProfanityFoundException(displayName);
    }

    return new(displayName);
  }

  public static implicit operator string(DisplayName displayName)
    => displayName.Value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }
}