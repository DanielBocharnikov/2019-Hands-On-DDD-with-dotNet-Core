using Marketplace.Framework;

namespace Marketplace.Domain.SharedCore;

public interface ICurrencyLookup
{
  CurrencyDetails FindCurrency(string currencyCode);
}

public class CurrencyDetails : ValueObject
{
  public string CurrencyCode { get; set; } = string.Empty;
  public bool InUse { get; set; }
  public int DecimalPlaces { get; set; }

  public static CurrencyDetails None => new()
  {
    CurrencyCode = string.Empty,
    InUse = false,
    DecimalPlaces = 0
  };

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return CurrencyCode;
    yield return InUse;
    yield return DecimalPlaces;
  }
}
