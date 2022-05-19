using Marketplace.Framework;

namespace Marketplace.Domain;

public sealed class ClassifiedAdText : ValueObject
{
  public static ClassifiedAdText FromString(string text) => new(text);

  private readonly string _value;

  internal ClassifiedAdText(string text) => _value = text;

  public static implicit operator string(ClassifiedAdText text) =>
    text._value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return _value;
  }
}