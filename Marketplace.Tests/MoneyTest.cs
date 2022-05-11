using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.Domain;
using Xunit;

namespace Marketplace.Tests;

public class FakeCurrencyLookup : ICurrencyLookup
{
  private static readonly IEnumerable<CurrencyDetails> currencies =
    new[]
    {
      new CurrencyDetails()
      {
        CurrencyCode = "EUR",
        DecimalPlaces = 2,
        InUse = true,
      },
      new CurrencyDetails()
      {
        CurrencyCode = "USD",
        DecimalPlaces = 2,
        InUse = true,
      },
      new CurrencyDetails()
      {
        CurrencyCode = "JPY",
        DecimalPlaces = 0,
        InUse = true,
      },
      new CurrencyDetails()
      {
        CurrencyCode = "DEM",
        DecimalPlaces = 2,
        InUse = false,
      },
    };

  public CurrencyDetails FindCurrency(string currencyCode)
  {
    CurrencyDetails? currency = currencies.FirstOrDefault(x =>
      x.CurrencyCode == currencyCode);

    return currency ?? CurrencyDetails.None;
  }
}

public class MoneyTest
{
  private static readonly ICurrencyLookup currencyLookup =
    new FakeCurrencyLookup();

  [Fact]
  public void TwoWithTheSameAmountShouldBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "EUR", currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", currencyLookup);

    Assert.Equal(firstAmount, secondAmount);
  }

  [Fact]
  public void TwoOfSameAmountButDifferentCurrenciesShouldNotBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "USD", currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", currencyLookup);

    Assert.NotEqual(firstAmount, secondAmount);
  }

  [Fact]
  public void SumOfMoneyGivesFullAmount()
  {
    var coin1 = Money.FromDecimal(1, "EUR", currencyLookup);
    var coin2 = Money.FromDecimal(2, "EUR", currencyLookup);
    var coin3 = Money.FromDecimal(2, "EUR", currencyLookup);

    var banknote = Money.FromDecimal(5, "EUR", currencyLookup);

    Assert.Equal(coin1 + coin2 + coin3, banknote);
  }

  [Fact]
  public void FromStringAndFromDecimalShouldBeEqual()
  {
    var firstAmount = Money.FromDecimal(5, "EUR", currencyLookup);
    var secondAmount = Money.FromString("5.00", "EUR", currencyLookup);

    Assert.Equal(firstAmount, secondAmount);
  }

  [Fact]
  public void UnusedCurrencyShouldNotBeAllowed() =>
    _ = Assert.Throws<ArgumentException>(() =>
      Money.FromDecimal(100, "DEM", currencyLookup));

  [Fact]
  public void UnknownCurrencyShouldNotBeAllowed() =>
    _ = Assert.Throws<ArgumentException>(() =>
      Money.FromDecimal(100, "What", currencyLookup));

  [Fact]
  public void ThrowWhenTooManyDecimalPlaces() =>
    _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
      Money.FromDecimal(100.123m, "EUR", currencyLookup));

  [Fact]
  public void ThrowsOnAddingDifferentCurrencies()
  {
    var firstAmount = Money.FromDecimal(5, "USD", currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", currencyLookup);

    _ = Assert.Throws<CurrencyMismatchException>(() =>
      firstAmount + secondAmount);
  }

  [Fact]
  public void ThrowsOnSubstractingDifferentCurrencies()
  {
    var firstAmount = Money.FromDecimal(5, "USD", currencyLookup);
    var secondAmount = Money.FromDecimal(5, "EUR", currencyLookup);

    _ = Assert.Throws<CurrencyMismatchException>(() =>
      firstAmount - secondAmount);
  }
}
