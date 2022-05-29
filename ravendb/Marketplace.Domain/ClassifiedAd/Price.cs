using Marketplace.Domain.SharedCore;

namespace Marketplace.Domain.ClassifiedAd;

public sealed class Price : Money
{
  public static readonly Price None = new()
  {
    Amount = -1,
    Currency = CurrencyDetails.None
  };

  /// <summary>
  /// Ctor for reapplying the events from aggregate root.
  /// </summary>
  /// <param name="amount"></param>
  /// <param name="currencyCode"></param>
  /// <param name="inUse"></param>
  /// <param name="decimalPlaces"></param>
  internal Price(
    decimal amount,
    string currencyCode,
    bool inUse,
    int decimalPlaces)
      : base(amount, new CurrencyDetails(currencyCode, inUse, decimalPlaces))
  {
  }

  /// <summary>
  /// Ctor called by factory method
  /// </summary>
  /// <param name="amount"></param>
  /// <param name="currencyCode"></param>
  /// <param name="currencyLookup"></param>
  private Price(
    decimal amount,
    string currencyCode,
    ICurrencyLookup currencyLookup) : base(amount, currencyCode, currencyLookup)
  {
  }

  /// <summary>
  /// Ctor that satisfies serialization requirements
  /// </summary>
  private Price() { }

  /// <summary>
  /// Factory method encapsulates validation and specific creation of an object.
  /// public new static to not hide Money method FromDecimal
  /// </summary>
  /// <param name="amount"></param>
  /// <param name="currencyCode"></param>
  /// <param name="currencyLookup"></param>
  /// <returns>Price</returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
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
}
