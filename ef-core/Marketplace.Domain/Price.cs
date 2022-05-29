namespace Marketplace.Domain;

public sealed class Price : Money
{
  public static readonly Price NoPrice = new()
  {
    Amount = -1,
    Currency = CurrencyDetails.None
  };

  public static new Price FromDecimal(
    decimal amount, string currencyCode, ICurrencyLookup currencyLookup)
  {
    if (amount < 0)
    {
      throw new ArgumentOutOfRangeException(
        paramName: nameof(amount),
        message: "Price cannot be negative"
      );
    }

    return new(amount, currencyCode, currencyLookup);
  }

  private Price()
  {
  }

  private Price(
    decimal amount,
    string currencyCode,
    ICurrencyLookup currencyLookup)
      : base(amount, currencyCode, currencyLookup)
  {
  }

  internal Price(
    decimal amount,
    string currencyCode,
    bool inUse,
    int decimalPlaces)
    : base(amount, new CurrencyDetails
    {
      CurrencyCode = currencyCode,
      InUse = inUse,
      DecimalPlaces = decimalPlaces
    })
  {
  }
}
