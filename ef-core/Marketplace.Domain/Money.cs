using System.Globalization;
using Marketplace.Framework;

namespace Marketplace.Domain;

public class Money : ValueObject
{
  public decimal Amount { get; init; }
  public CurrencyDetails Currency { get; init; }

  public const string DEFAULTCURRENCY = "EUR";

  public static Money FromDecimal(
    decimal amount,
    string currencyCode,
    ICurrencyLookup currencyLookup) =>
      new(amount, currencyCode, currencyLookup);

  public static Money FromString(
    string amount,
    string currencyCode,
    ICurrencyLookup currencyLookup) =>
      new(Convert.ToDecimal(amount, new CultureInfo("en-US")),
        currencyCode, currencyLookup);

  protected Money(
      decimal amount,
      string currencyCode,
      ICurrencyLookup currencyLookup)
  {
    if (string.IsNullOrEmpty(currencyCode))
    {
      throw new ArgumentNullException(
      paramName: nameof(currencyCode),
      message: "Currency code must be specified"
      );
    }

    CurrencyDetails currency = currencyLookup.FindCurrency(currencyCode);

    if (!currency.InUse)
    {
      throw new ArgumentException(
      message: $"Currency {currencyCode} is not valid"
      );
    }

    if (decimal.Round(amount, currency.DecimalPlaces) != amount)
    {
      throw new ArgumentOutOfRangeException(
      paramName: nameof(amount),
      message: $"Amount in {currencyCode} cannot have more than {currency.DecimalPlaces} decimals"
      );
    }

    Amount = amount;
    Currency = currency;
  }

  protected Money(decimal amount, CurrencyDetails currency)
  {
    Amount = amount;
    Currency = currency;
  }

  public Money Add(Money summand)
  {
    if (Currency != summand.Currency)
    {
      throw new CurrencyMismatchException(
      message: "Cannot sum amounts with different currencies"
      );
    }

    return new Money(Amount + summand.Amount, Currency);
  }

  public Money Subtract(Money subtrahend)
  {
    if (Currency != subtrahend.Currency)
    {
      throw new CurrencyMismatchException(
      message: "Cannot subtract amounts with different currencies"
      );
    }

    return new Money(Amount - subtrahend.Amount, Currency);
  }

  public static Money operator +(Money summand1, Money summand2) =>
  summand1.Add(summand2);

  public static Money operator -(Money minuend, Money subtrahend) =>
  minuend.Subtract(subtrahend);

  public override string ToString() => $"{Currency.CurrencyCode} {Amount}";

  protected override IEnumerable<object> GetEqualityComponents()
  {
    yield return Amount;
    yield return Currency;
  }
}
