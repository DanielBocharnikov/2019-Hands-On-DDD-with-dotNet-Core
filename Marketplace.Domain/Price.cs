namespace Marketplace.Domain;

public sealed class Price : Money
{
  public static new Price FromDecimal(
    decimal amount, string currencyCode, ICurrencyLookup currencyLookup) =>
      new(amount, currencyCode, currencyLookup);

  private Price(
    decimal amount,
    string currencyCode,
    ICurrencyLookup currencyLookup)
      : base(amount, currencyCode, currencyLookup)
  {
    if (amount < 0)
    {
      throw new ArgumentOutOfRangeException(
        paramName: nameof(amount),
        message: "Price cannot be negative"
      );
    }
  }

  private Price(decimal amount, string currencyCode)
    : base(amount, new CurrencyDetails { CurrencyCode = currencyCode })
  {
  }
}
