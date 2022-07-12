using Marketplace.Framework;

namespace Marketplace.Domain.SharedCore;

public class CurrencyDetails : ValueObject
{
  public string CurrencyCode { get; private set; } = string.Empty;
  public bool InUse { get; private set; }
  public int DecimalPlaces { get; private set; }

  public static CurrencyDetails None => new()
  {
    CurrencyCode = string.Empty,
    InUse = false,
    DecimalPlaces = default
  };

  public CurrencyDetails(string currencyCode, bool inUse, int decimalPlaces)
  {
    CurrencyCode = currencyCode;
    InUse = inUse;
    DecimalPlaces = decimalPlaces;
  }

  private CurrencyDetails()
  {
  }

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return CurrencyCode;
    yield return InUse;
    yield return DecimalPlaces;
  }
}
