using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class ClassifiedAdText : ValueObject
{
  public static ClassifiedAdText FromString(string text) => new(text);

  public string Value { get; init; } = string.Empty;

  internal ClassifiedAdText(string text) => Value = text;

  public static implicit operator string(ClassifiedAdText text) =>
    text.Value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }

  private ClassifiedAdText() { }
}