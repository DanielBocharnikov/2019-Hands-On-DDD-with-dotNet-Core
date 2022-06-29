namespace Marketplace.Domain.SharedCore;

public interface ICurrencyLookup
{
  CurrencyDetails FindCurrency(string currencyCode);
}
