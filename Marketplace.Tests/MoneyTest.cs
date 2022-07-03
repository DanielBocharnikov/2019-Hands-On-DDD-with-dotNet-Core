using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain.SharedCore;
using Xunit;

namespace Marketplace.Tests;

public class FakeCurrencyLookup : ICurrencyLookup
{
  private static readonly IEnumerable<CurrencyDetails> _currencies =
    new[]
    {
      new CurrencyDetails(currencyCode: "EUR", inUse: true, decimalPlaces: 2),
      new CurrencyDetails(currencyCode: "USD", inUse: true, decimalPlaces: 2),
      new CurrencyDetails(currencyCode: "JPY", inUse: true, decimalPlaces: 0),
      new CurrencyDetails(currencyCode: "DEM", inUse: false, decimalPlaces: 2),
    };

  public CurrencyDetails FindCurrency(string currencyCode) => _currencies
    .FirstOrDefault(x => x.CurrencyCode == currencyCode)
      ?? CurrencyDetails.None;
}

public class MoneyTest
{
  private static readonly ICurrencyLookup _currencyLookup =
    new FakeCurrencyLookup();

  [Fact]
  public void TwoWithTheSameAmountShouldBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "EUR", _currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", _currencyLookup);

    Assert.Equal(firstAmount, secondAmount);
  }

  [Fact]
  public void TwoOfSameAmountButDifferentCurrenciesShouldNotBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "USD", _currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", _currencyLookup);

    Assert.NotEqual(firstAmount, secondAmount);
  }

  [Fact]
  public void SumOfMoneyGivesFullAmount()
  {
    var coin1 = Money.FromDecimal(1, "EUR", _currencyLookup);
    var coin2 = Money.FromDecimal(2, "EUR", _currencyLookup);
    var coin3 = Money.FromDecimal(2, "EUR", _currencyLookup);

    var banknote = Money.FromDecimal(5, "EUR", _currencyLookup);

    Assert.Equal(coin1 + coin2 + coin3, banknote);
  }

  [Fact]
  public void FromStringAndFromDecimalShouldBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "EUR", _currencyLookup);
    var secondAmount = Money.FromString("5.00", "EUR", _currencyLookup);

    Assert.Equal(firstAmount, secondAmount);
  }

  [Fact]
  public void UnusedCurrencyShouldNotBeAllowed() =>
    _ = Assert.Throws<ArgumentException>(() =>
      Money.FromDecimal(100, "DEM", _currencyLookup));

  [Fact]
  public void UnknownCurrencyShouldNotBeAllowed() =>
    _ = Assert.Throws<ArgumentException>(() =>
      Money.FromDecimal(100, "What", _currencyLookup));

  [Fact]
  public void ThrowWhenTooManyDecimalPlaces() =>
    _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
      Money.FromDecimal(100.123m, "EUR", _currencyLookup));

  [Fact]
  public void ThrowsOnAddingDifferentCurrencies()
  {
    var firstAmount = Money.FromDecimal(5, "USD", _currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", _currencyLookup);

    _ = Assert.Throws<DomainExceptions.CurrencyMismatchException>(() =>
      firstAmount + secondAmount);
  }

  [Fact]
  public void ThrowsOnSubstractingDifferentCurrencies()
  {
    var firstAmount = Money.FromDecimal(5, "USD", _currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", _currencyLookup);

    _ = Assert.Throws<DomainExceptions.CurrencyMismatchException>(() =>
      firstAmount - secondAmount);
  }
}
