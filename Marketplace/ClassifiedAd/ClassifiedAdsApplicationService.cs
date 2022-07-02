using Marketplace.Domain.ClassifiedAd;
using Marketplace.Domain.SharedCore;
using Marketplace.Framework;
using static Marketplace.ClassifiedAd.Commands;

namespace Marketplace.ClassifiedAd;

public class ClassifiedAdsApplicationService : IApplicationService
{
  private readonly ICurrencyLookup _currencyLookup;
  private readonly IAggregateStore _store;

  public ClassifiedAdsApplicationService(IAggregateStore store,
    ICurrencyLookup currencyLookup)
  {
    _store = store;
    _currencyLookup = currencyLookup;
  }

  public Task Handle(object command) =>
    command switch
    {
      V1.Create cmd => HandleCreate(cmd),

      V1.SetTitle cmd => HandleUpdate(
        cmd.Id,
        ad => ad.SetTitle(ClassifiedAdTitle.FromString(cmd.Title))
      ),

      V1.UpdateText cmd => HandleUpdate(
        cmd.Id,
        ad => ad.UpdateText(ClassifiedAdText.FromString(cmd.Text))
      ),

      V1.UpdatePrice cmd => HandleUpdate(
        cmd.Id,
        ad => ad.UpdatePrice(
          Price.FromDecimal(cmd.Price, cmd.CurrencyCode, _currencyLookup)
        )
      ),

      V1.AddPicture cmd => HandleUpdate(
        cmd.Id,
        ad => ad.AddPicture(
          new Uri(cmd.PictureUrl),
          new PictureSize(cmd.Height, cmd.Width)
        )
      ),

      V1.ResizePicture cmd => HandleUpdate(
        cmd.Id,
        ad => ad.ResizePicture(
          new PictureId(cmd.PictureId),
          new PictureSize(cmd.Height, cmd.Width)
        )
      ),

      V1.RequestToPublish cmd => HandleUpdate(
        cmd.Id,
        ad => ad.RequestToPublish()),

      V1.Publish cmd => HandleUpdate(
        cmd.Id,
        c => c.Publish(new UserId(cmd.ApprovedBy))),

      _ => throw new InvalidOperationException(
          $"Command type {command.GetType().FullName} is unknown."
        ),
    };

  private async Task HandleCreate(V1.Create cmd)
  {
    if (await _store.Exists<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(
      (ClassifiedAdId)cmd.Id))
    {
      throw new InvalidOperationException($"Entity with id {cmd.Id}," +
        " already exists");
    }

    var classifiedAd = new Domain.ClassifiedAd.ClassifiedAd(
      (ClassifiedAdId)cmd.Id,
      (UserId)cmd.OwnerId
    );

    await _store
      .Save<Domain.ClassifiedAd.ClassifiedAd, ClassifiedAdId>(classifiedAd);
  }

  private async Task HandleUpdate(
    Guid id,
    Action<Domain.ClassifiedAd.ClassifiedAd> operation)
      => await this.HandleUpdate(_store, (ClassifiedAdId)id, operation);
}