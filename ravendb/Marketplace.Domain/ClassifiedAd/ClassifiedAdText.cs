using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class ClassifiedAdText : ValueObject
{
  public static ClassifiedAdText None => new(string.Empty);

  public string Value { get; internal set; } = string.Empty;

  internal ClassifiedAdText(string text) => Value = text;

  /// <summary>
  /// Satisfies serialization requirements.
  /// </summary>
  private ClassifiedAdText()
  {
  }

  public static ClassifiedAdText FromString(string text) => new(text);

  public static implicit operator string(ClassifiedAdText text)
    => text.Value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }
}