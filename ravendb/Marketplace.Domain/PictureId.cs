using Marketplace.Framework;

namespace Marketplace.Domain;

public sealed class PictureId : ValueObject
{
  public Guid Value { get; init; }

  public PictureId(Guid value) => Value = value;

  public static implicit operator Guid(PictureId self) => self.Value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return Value;
  }
}