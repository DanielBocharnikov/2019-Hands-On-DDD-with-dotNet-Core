using System;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Xunit;

namespace Marketplace.Tests;

public class ClassifiedAdPublishSpec
{
  private readonly ClassifiedAd _classifiedAd;
  private readonly UserId _approvedByUserId;

  public ClassifiedAdPublishSpec()
  {
    _classifiedAd = new(
      new ClassifiedAdId(Guid.NewGuid()),
      new UserId(Guid.NewGuid())
      );

    _approvedByUserId = new(Guid.NewGuid());
  }

  [Fact]
  public void CanRequestToPublishAValidAd()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));
    _classifiedAd.RequestToPublish();

    Assert.Equal(
      ClassifiedAd.ClassifiedAdState.PendingReview, _classifiedAd.State);
  }

  [Fact]
  public void CannotRequestToPublishWithoutTitle()
  {
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotRequestToPublishWithoutText()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotRequestToPublishWithoutPrice()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotRequestToPublishWithZeroPrice()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(0.0m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotRequestToPublishWithoutOnePicture()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CannotRequestToPublishIfPictureIsNot800x600Size()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(200, 300));

    _ = Assert.Throws<DomainExceptions.InvalidEntityStateException>(() =>
      _classifiedAd.RequestToPublish());
  }

  [Fact]
  public void CanPublishAValidAd()
  {
    _classifiedAd.SetTitle(
      ClassifiedAdTitle.FromString("Test ad"));
    _classifiedAd.UpdateText(
      ClassifiedAdText.FromString("Please buy my stuff"));
    _classifiedAd.UpdatePrice(
      Price.FromDecimal(100.10m, "EUR", new FakeCurrencyLookup()));
    _classifiedAd.AddPicture(new Uri("about:blank"), new PictureSize(600, 800));
    _classifiedAd.RequestToPublish();
    _classifiedAd.Publish(_approvedByUserId);

    Assert.Equal(
      ClassifiedAd.ClassifiedAdState.Active, _classifiedAd.State);
  }
}