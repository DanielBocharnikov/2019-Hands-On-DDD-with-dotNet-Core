using Marketplace.Domain.SharedCore;

namespace Marketplace.Infrastructure;

public class FixedCurrencyLookup : ICurrencyLookup
{
  private static readonly IEnumerable<CurrencyDetails> _currencies =
      new[]
      {
              new CurrencyDetails("EUR", true, 2),
              new CurrencyDetails("USD", true, 2)
      };

  public CurrencyDetails FindCurrency(string currencyCode) => _currencies
      .FirstOrDefault(x => x.CurrencyCode == currencyCode)
        ?? CurrencyDetails.None;
}