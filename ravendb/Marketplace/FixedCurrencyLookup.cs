using Marketplace.Domain;

namespace Marketplace;

public class FixedCurrencyLookup : ICurrencyLookup
{
  private static readonly IEnumerable<CurrencyDetails> _currencies =
      new[]
      {
              new CurrencyDetails
              {
                  CurrencyCode = "EUR",
                  DecimalPlaces = 2,
                  InUse = true
              },
              new CurrencyDetails
              {
                  CurrencyCode = "USD",
                  DecimalPlaces = 2,
                  InUse = true
              }
      };

  public CurrencyDetails FindCurrency(string currencyCode)
  {
    CurrencyDetails? currency = _currencies.FirstOrDefault(x =>
      x.CurrencyCode == currencyCode);
    return currency ?? CurrencyDetails.None;
  }
}