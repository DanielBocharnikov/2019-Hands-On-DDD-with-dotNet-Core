using Marketplace.Framework;

namespace Marketplace.Domain;

public interface ICurrencyLookup
{
  CurrencyDetails FindCurrency(string currencyCode);
}

public class CurrencyDetails : ValueObject
{
  public string? CurrencyCode { get; set; }
  public bool InUse { get; set; }
  public int DecimalPlaces { get; set; }

  public static CurrencyDetails None => new() { InUse = false };

  protected override IEnumerable<object?> GetEqualityComponents()
  {
    yield return CurrencyCode;
    yield return InUse;
    yield return DecimalPlaces;
  }
}
