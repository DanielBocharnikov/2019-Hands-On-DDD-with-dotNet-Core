using System;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Xunit;

namespace Marketplace.Tests;

public class ClassifiedAdPublishSpec
{
  private readonly ClassifiedAd _classifiedAd;

  public ClassifiedAdPublishSpec() =>
    _classifiedAd = new(
      new ClassifiedAdId(Guid.NewGuid()),
      new UserId(Guid.NewGuid())
    );

  [Fact]
  public void CanPublishAValidAd()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.RequestToPublish();

    Assert.Equal(
      ClassifiedAd.ClassifiedAdState.PendingReview, _classifiedAd.State);
  }

  [Fact]
  public void CannotPublishWithoutTitle()
  {
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotPublishWithoutText()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotPublishWithoutPrice()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotPublishWithZeroPrice()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(0.0m, "EUR", new FakeCurrencyLookup()));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }
}