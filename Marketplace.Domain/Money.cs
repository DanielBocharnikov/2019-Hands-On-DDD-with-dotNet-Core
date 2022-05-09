using Marketplace.Framework;

namespace Marketplace.Domain
{
  public class Money : ValueObject
  {
    private const string DefaultCurrency = "EUR";
    public decimal Amount { get; init; }
    public string CurrencyCode { get; init; }

    public static Money FromDecimal(decimal amount, string currency = DefaultCurrency) =>
      new Money(amount, currency);

    public static Money FromString(string amount, string currency = DefaultCurrency) =>
      new Money(decimal.Parse(amount), currency);

    protected Money(decimal amount, string currency = "EUR")
    {
      if (decimal.Round(amount, 2) != amount)
      {
        throw new ArgumentOutOfRangeException(
          paramName: nameof(amount),
          message: "Amount cannot have more than two decimals"
        );
      }
      Amount = amount;
      CurrencyCode = currency;
    }

    public Money Add(Money summand)
    {
      if (CurrencyCode != summand.CurrencyCode)
      {
        throw new CurrencyMismatchException(
          message: "Cannot sum amounts with different currencies"
        );
      }

      return new Money(Amount + summand.Amount);
    }

    public Money Subtract(Money subtrahend)
    {
      if (CurrencyCode != subtrahend.CurrencyCode)
      {
        throw new CurrencyMismatchException(
          message: "Cannot subtract amounts with different currencies"
        );
      }

      return new Money(Amount - subtrahend.Amount);
    }

    public static Money operator +(Money summand1, Money summand2) =>
      summand1.Add(summand2);

    public static Money operator -(Money minuend, Money subtrahend) =>
      minuend.Subtract(subtrahend);

    protected override IEnumerable<object> GetEqualityComponents()
    {
      yield return Amount;
      yield return CurrencyCode;
    }
  }

  public class CurrencyMismatchException : Exception
  {
    public Money(string message) : base(message)
    {
    }
  }
}
