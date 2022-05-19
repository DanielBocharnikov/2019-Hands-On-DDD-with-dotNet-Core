using Marketplace.Framework;

namespace Marketplace.Domain;

public class PictureId : ValueObject
{
  private readonly Guid _value;

  public PictureId(Guid value) => _value = value;

  public static implicit operator Guid(PictureId self) => self._value;

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return _value;
  }
}